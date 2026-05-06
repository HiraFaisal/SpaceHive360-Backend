using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpaceHive360.Member.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Infrastructure.Services
{
    public interface IInventoryService
    {
        Task UpdateWorkspaceAvailabilityAsync(Guid workspaceId);
    }

    public class InventoryService : IInventoryService
    {
        private readonly MemberDbContext _context;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(MemberDbContext context, ILogger<InventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task UpdateWorkspaceAvailabilityAsync(Guid workspaceId)
        {
            var workspace = await _context.Workspaces.FindAsync(workspaceId);
            if (workspace == null) return;

            // Determine Limit
            int limit = workspace.InventoryType == "UNIT" ? 1 : (workspace.MaxUnits > 0 ? workspace.MaxUnits : (workspace.Capacity ?? 1));

            // Count Active or Pending memberships
            var now = DateTime.UtcNow;
            var activeCount = await (from m in _context.MemberMemberships
                                   join p in _context.PlanMemberships on m.FkPlan equals p.RecId
                                   where p.FkWorkspace == workspaceId 
                                   && (m.MembershipStatus == "Active" || m.MembershipStatus == "Pending")
                                   && m.EndDate > now
                                   select m).CountAsync();

            _logger.LogInformation($"Recalculating availability for Workspace: {workspace.Name} (ID: {workspaceId}). Usage: {activeCount}/{limit}.");

            if (activeCount >= limit)
            {
                workspace.IsAvailable = false;
                
                // Also disable all associated membership plans
                var plans = await _context.PlanMemberships.Where(p => p.FkWorkspace == workspaceId).ToListAsync();
                foreach (var p in plans) p.IsActive = false;

                await _context.SaveChangesAsync();
                
                // Console log as requested (plus logger)
                Console.WriteLine($"[NOTIFICATION] Workspace {workspace.Name} (ID: {workspaceId}) is now FULL ({activeCount}/{limit}). Availability disabled.");
                _logger.LogWarning($"Workspace {workspace.Name} is now FULL. Availability disabled.");
            }
            else
            {
                // Re-enable if it was disabled but now has slots
                if (!workspace.IsAvailable)
                {
                    workspace.IsAvailable = true;
                    var plans = await _context.PlanMemberships.Where(p => p.FkWorkspace == workspaceId).ToListAsync();
                    foreach (var p in plans) p.IsActive = true;
                    
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"[NOTIFICATION] Workspace {workspace.Name} (ID: {workspaceId}) has slots available. Re-enabled.");
                }
            }
        }
    }
}
