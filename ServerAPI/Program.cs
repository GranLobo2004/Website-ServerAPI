using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar el servicio de contexto de la base de datos
builder.Services.AddDbContext<DataBase>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar FastEndpoints y Swagger
builder.Services.AddFastEndpoints().SwaggerDocument();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseFastEndpoints().UseSwaggerGen();
app.Run();