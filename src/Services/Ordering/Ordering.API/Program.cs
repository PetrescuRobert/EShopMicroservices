var builder = WebApplication.CreateBuilder(args);

// Add services to the IoC container

var app = builder.Build();

// Configure the HTTP request pipeline

app.Run();
