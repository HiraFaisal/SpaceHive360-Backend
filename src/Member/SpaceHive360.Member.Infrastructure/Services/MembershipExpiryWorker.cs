using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SpaceHive360.Member.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Infrastructure.Services
{
    public class MembershipExpiryWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MembershipExpiryWorker> _logger;

        public MembershipExpiryWorker(IServiceProvider serviceProvider, ILogger<MembershipExpiryWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Membership Expiry Worker is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessExpiriesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing membership expiries.");
                }

                // Run every hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task ProcessExpiriesAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<MemberDbContext>();
                var now = DateTime.UtcNow;

                // 1. Find Expired Memberships
                var expiredMemberships = await context.MemberMemberships
                    .Where(m => m.MembershipStatus == "Active" && m.EndDate < now)
                    .ToListAsync();

                if (!expiredMemberships.Any()) return;

                _logger.LogInformation($"Found {expiredMemberships.Count} memberships to expire.");

                // Track which workspaces might need re-enabling
                var affectedWorkspaceIds = await (from m in context.MemberMemberships
                                               join p in context.PlanMemberships on m.FkPlan equals p.RecId
                                               where expiredMemberships.Select(em => em.RecId).Contains(m.RecId)
                                               select p.FkWorkspace).Distinct().ToListAsync();

                foreach (var membership in expiredMemberships)
                {
                    membership.MembershipStatus = "Expired";
                }

                await context.SaveChangesAsync();

                var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();

                // 2. Re-enable Availability if count < limit
                foreach (var workspaceId in affectedWorkspaceIds)
                {
                    if (workspaceId.HasValue)
                    {
                        await inventoryService.UpdateWorkspaceAvailabilityAsync(workspaceId.Value);
                    }
                }
            }
        }
    }
}
