using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints().SwaggerDocument();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseFastEndpoints().UseSwaggerGen();
app.Run();