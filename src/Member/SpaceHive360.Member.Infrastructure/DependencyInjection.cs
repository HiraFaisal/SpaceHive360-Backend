using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpaceHive360.Member.Application.Services;
using SpaceHive360.Member.Infrastructure.Data;
using SpaceHive360.Member.Infrastructure.Services;

namespace SpaceHive360.Member.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var postgresConnection = Environment.GetEnvironmentVariable("PostgreConnection");

            services.AddDbContext<MemberDbContext>(options =>
                options.UseNpgsql(postgresConnection)
            );

            services.AddScoped<IMemberPortalService, MemberPortalService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPaymentService, PaymentService>();

            return services;
        }
    }
}
