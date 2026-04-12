using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpaceHive360.Admin.Application.Security;
using SpaceHive360.Admin.Application.Security.JwtToken;
using SpaceHive360.Admin.Application.Services.AdminUsers;
using SpaceHive360.Admin.Application.Services.Auth;
using SpaceHive360.Admin.Application.Services.Files;
using SpaceHive360.Admin.Application.Services.Plans;
using SpaceHive360.Admin.Application.Services.PaymentTerms;
using SpaceHive360.Admin.Application.Services.Workspace;
using SpaceHive360.Admin.Application.Services.WorkspaceType;
using SpaceHive360.Admin.Infrastructure.Services;

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
            services.AddScoped<IWorkspaceTypeService, WorkspaceTypeService>();
            services.AddScoped<IWorkspaceService, WorkspaceService>();
            services.AddScoped<IPaymentTermsService, PaymentTermsService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IPlanService, PlanService>();
            // ---------------------------
            // Configure JWT Settings
            // ---------------------------
            // Bind JWT settings from configuration (appsettings.json or env)
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            return services;
        }
    }
}
