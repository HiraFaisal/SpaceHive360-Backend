using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpaceHive360.Admin.Application.Security;
using SpaceHive360.Admin.Application.Security.JwtToken;
using SpaceHive360.Admin.Application.Services.AdminUsers;
using SpaceHive360.Admin.Application.Services.Auth;

namespace SpaceHive360.Admin.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services, IConfiguration configuration)
        {
            // ---------------------------
            // Register your application services
            // ---------------------------
            services.AddScoped<IAdminUserService, AdminUserService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // ---------------------------
            // Configure JWT Settings
            // ---------------------------
            // Bind JWT settings from configuration (appsettings.json or env)
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            return services;
        }
    }
}
