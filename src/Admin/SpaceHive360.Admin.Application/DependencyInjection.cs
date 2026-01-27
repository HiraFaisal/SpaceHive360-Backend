using Microsoft.Extensions.DependencyInjection;
using SpaceHive360.Admin.Application.Services;
using SpaceHive360.Admin.Application.Services.AdminUsers;

namespace SpaceHive360.Admin.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            // Register your application services here
            services.AddScoped<IAdminUserService, AdminUserService>();

            // If you have other services in future, add them here:
            // services.AddScoped<IOtherService, OtherService>();

            return services;
        }
    }
}
