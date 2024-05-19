using BuildingBlocks.Exceptions.Handler;
using HealthChecks.UI.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

// Add services to the IoC container
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Database")!);
    options.Schema.For<ShoppingCart>().Identity(member => member.UserName);
}).UseLightweightSessions();


// Register a decorated repository
// Also could be done with Scrutor library
builder.Services.AddScoped<IBasketRepository>(provider =>
{
    var basketRepository = provider.GetRequiredService<BasketRepository>();
    var cache = provider.GetRequiredService<IDistributedCache>();

    return new CachedBasketRepository(basketRepository, cache);
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

// Register Healtch Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);


var app = builder.Build();

// Configure the HTTP request pipline
app.MapCarter();

app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
