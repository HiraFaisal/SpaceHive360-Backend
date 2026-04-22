using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpaceHive360.SuperAdmin.Application.Interfaces;
using SpaceHive360.SuperAdmin.Application.Security;
using SpaceHive360.SuperAdmin.Application.Security.JwtToken;
using SpaceHive360.SuperAdmin.Application.Services;

namespace SpaceHive360.SuperAdmin.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSuperAdminApplicationDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISuperAdminAuthService, SuperAdminAuthService>();
            services.AddScoped<ISuperAdminUserService, SuperAdminUserService>();
            services.AddScoped<ISuperAdminCompanyService, SuperAdminCompanyService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            return services;
        }
    }
}
