using BuildingBlocks.Exceptions.Handler;
using BuildingBlocks.Messaging;
using Discount.gRPC;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

// Add services to the IoC container

// Application Services
#region Application Services

builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

#endregion

// Data Services
#region Data Services
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Database")!);
    options.Schema.For<ShoppingCart>().Identity(member => member.UserName);
}).UseLightweightSessions();

//builder.Services.AddScoped<IBasketRepository>(provider =>
//{
//    // Register a decorated repository
//    // Also could be done with Scrutor library
//    var basketRepository = provider.GetRequiredService<BasketRepository>();
//    var cache = provider.GetRequiredService<IDistributedCache>();

//    return new CachedBasketRepository(basketRepository, cache);
//});
builder.Services.AddScoped<IBasketRepository, CachedBasketRepository>();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

#endregion

// gRpc Services
#region gRPC Services
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
})
    //Not recommanded in production
    .ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
    return handler;
});

#endregion


#region Async Communication Services
builder.Services.AddMessageBroker(builder.Configuration);
#endregion

// Cross-Cutting Services
#region Cross-Cutting Services

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

// Register Healtch Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

#endregion

var app = builder.Build();

// Configure the HTTP request pipline
app.MapCarter();

app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
