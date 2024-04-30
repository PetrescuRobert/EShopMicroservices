var builder = WebApplication.CreateBuilder(args);

// Add services to the IOC container

var app = builder.Build();

// Configure the HTTP request pipeline

app.Run();
