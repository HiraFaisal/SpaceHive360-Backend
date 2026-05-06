using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpaceHive360.Member.Application.Models;
using SpaceHive360.Member.Application.Services;
using SpaceHive360.Member.Domain.Entities;
using SpaceHive360.Member.Infrastructure.Data;

namespace SpaceHive360.Member.Infrastructure.Services
{
    public class UserActivityService : IUserActivityService
    {
        private readonly MemberDbContext _context;

        public UserActivityService(MemberDbContext context)
        {
            _context = context;
        }

        public async Task LogActivityAsync(UserActivityRequest request)
        {
            var activity = new UserActivity
            {
                FkMember = request.MemberId ?? Guid.Empty,
                WorkspaceId = request.WorkspaceId,
                ActionType = request.ActionType,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserActivities.Add(activity);
            await _context.SaveChangesAsync();
        }

        public async Task LogActivitiesBatchAsync(List<UserActivityRequest> requests)
        {
            if (requests == null || !requests.Any()) return;

            var activities = requests.Select(r => new UserActivity
            {
                FkMember = r.MemberId ?? Guid.Empty,
                WorkspaceId = r.WorkspaceId,
                ActionType = r.ActionType,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _context.UserActivities.AddRange(activities);
            await _context.SaveChangesAsync();
        }
    }
}
