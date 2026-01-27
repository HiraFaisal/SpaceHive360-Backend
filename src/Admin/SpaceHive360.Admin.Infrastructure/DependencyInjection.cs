using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpaceHive360.Admin.Domain.IRepositories;
using SpaceHive360.Admin.Infrastructure.Data;
using SpaceHive360.Admin.Infrastructure.Repositories;

namespace SpaceHive360.Admin.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 🔹 Get connection from ENV
            var postgresConnection = Environment.GetEnvironmentVariable("PostgreConnection");

            services.AddDbContext<AdminDbContext>(options =>
                options.UseNpgsql(postgresConnection)
            );
            services.AddScoped<IAdminUserRepository, AdminUserRepository>();

            return services;
        }
    }
}
