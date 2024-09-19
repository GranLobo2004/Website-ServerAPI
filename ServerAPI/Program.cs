using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using FastEndpoints.Security;


var builder = WebApplication.CreateBuilder(args);


// Agregar el servicio de contexto de la base de datos
builder.Services.AddDbContext<DataBase>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//LOG IN
/*
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .addEntityFrameworkStores<DataBase>()
    .AddDefaultTokenProviders();
*/

// Agregar FastEndpoints y Swagger
builder.Services.AddFastEndpoints().SwaggerDocument()
    .AddAuthenticationJwtBearer(s => s.SigningKey = "The secret used to sign tokens")
    .AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});


var app = builder.Build();

app.UseCors("AllowAll");

app.MapGet("/", () => "Hello World!");
app.UseFastEndpoints().UseSwaggerGen().UseAuthentication().UseAuthorization();
app.Run();