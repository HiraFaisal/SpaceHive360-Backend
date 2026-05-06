using SpaceHive360.Member.Infrastructure;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SpaceHive360 Member API",
        Version = "v1"
    });
});

// DI for Infrastructure (includes Application services)
builder.Services.AddInfrastructureDI(builder.Configuration);

var app = builder.Build();

// Serve static files from Admin API's wwwroot
// Correct path: Go up two levels (Member.Api -> Member -> src) then into Admin/SpaceHive360.Admin.Api/wwwroot
var adminWwwRoot = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "..", "Admin", "SpaceHive360.Admin.Api", "wwwroot"));
if (Directory.Exists(adminWwwRoot))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(adminWwwRoot),
        RequestPath = "" 
    });
}

// Configure the HTTP request pipeline.
// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SpaceHive360 Member API V1");
    options.RoutePrefix = string.Empty;
});

app.UseCors("AllowFrontend");
// Only use HTTPS redirect in production with proper SSL
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
app.UseAuthorization();
app.MapControllers();

app.Run();
