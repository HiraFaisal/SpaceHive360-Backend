using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpaceHive360.SuperAdmin.Domain.IRepositories;
using SpaceHive360.SuperAdmin.Infrastructure.Data;
using SpaceHive360.SuperAdmin.Infrastructure.Repositories;

namespace SpaceHive360.SuperAdmin.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSuperAdminInfrastructureDI(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 🔹 Get connection from ENV
            var postgresConnection = Environment.GetEnvironmentVariable("PostgreConnection");

            services.AddDbContext<SuperAdminDbContext>(options =>
                options.UseNpgsql(postgresConnection)
            );

            services.AddScoped<ISuperAdminUserRepository, SuperAdminUserRepository>();
            services.AddScoped<ISuperAdminCompanyRepository, SuperAdminCompanyRepository>();
            services.AddScoped<SpaceHive360.SuperAdmin.Application.Interfaces.ISupportService, SpaceHive360.SuperAdmin.Infrastructure.Services.SupportService>();

            return services;
        }
    }
}
