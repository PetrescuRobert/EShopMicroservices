var builder = WebApplication.CreateBuilder(args);

// Add services to the IoC container

var app = builder.Build();

// Configure the HTTP request pipline

app.MapGet("/", () => "Hello World!");

app.Run();
