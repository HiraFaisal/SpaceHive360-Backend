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
using SpaceHive360.Admin.Application.Services.Location;
using SpaceHive360.Admin.Application.Services.Feedback;
using SpaceHive360.Admin.Application.Services.PlanBookings;
using SpaceHive360.Admin.Application.Services.PlanMemberships;
using SpaceHive360.Admin.Application.Services.Companies;

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
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IPlanBookingService, PlanBookingService>();
            services.AddScoped<IPlanMembershipService, PlanMembershipService>();
            services.AddScoped<ICompanyService, CompanyService>();
            // ---------------------------
            // Configure JWT Settings
            // ---------------------------
            // Bind JWT settings from configuration (appsettings.json or env)
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            return services;
        }
    }
}
