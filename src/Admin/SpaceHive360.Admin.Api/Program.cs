using SpaceHive360.Admin.Application;
using SpaceHive360.Admin.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// -------------------
// Add services to the container
// -------------------

// Add controllers
builder.Services.AddControllers();

// Add Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer(); // Needed for Swagger UI
builder.Services.AddSwaggerGen();

// -------------------
// Add PostgreSQL connection & DI
// -------------------

// 1. Infrastructure DI (DbContext + Repository)
builder.Services.AddInfrastructureDI(builder.Configuration);

// 2. Application DI (Services)
builder.Services.AddApplicationDI();

var app = builder.Build();

// -------------------
// Configure middleware pipeline
// -------------------

// Enable Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();            // Serve generated Swagger as JSON
    app.UseSwaggerUI(c =>        // Serve Swagger UI
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Admin API V1");
        c.RoutePrefix = string.Empty; // Open Swagger at root: https://localhost:5001/
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
