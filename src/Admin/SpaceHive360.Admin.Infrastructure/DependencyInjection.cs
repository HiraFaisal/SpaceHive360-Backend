using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpaceHive360.Admin.Domain.IRepositories;
using ApplicationRepo = SpaceHive360.Admin.Application.IRepositories;
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
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IWorkspaceTypeRepository, WorkspaceTypeRepository>();
            services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<IPaymentTermsRepository, PaymentTermsRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IPlanBookingRepository, PlanBookingRepository>();
            services.AddScoped<IPlanMembershipRepository, PlanMembershipRepository>();
            services.AddScoped<ApplicationRepo.IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<SpaceHive360.Admin.Application.Services.Bookings.IBookingService, SpaceHive360.Admin.Infrastructure.Services.Bookings.BookingService>();

            return services;
        }
    }
}
