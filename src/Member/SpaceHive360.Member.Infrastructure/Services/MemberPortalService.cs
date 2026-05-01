using Microsoft.EntityFrameworkCore;
using SpaceHive360.Member.Application.Models;
using SpaceHive360.Member.Application.Services;
using SpaceHive360.Member.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Infrastructure.Services
{
    public class MemberPortalService : IMemberPortalService
    {
        private readonly MemberDbContext _context;

        public MemberPortalService(MemberDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> GetCitiesAsync()
        {
            var cities = await _context.Cities
                .Where(c => c.IsActive)
                .Select(c => c.Name)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return ApiResponse.SuccessResponse(cities);
        }

        public async Task<ApiResponse> GetCategoriesAsync()
        {
            var categories = await _context.WorkspaceTypes
                .Where(t => t.IsActive)
                .Select(t => t.Name)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            return ApiResponse.SuccessResponse(categories);
        }

        public async Task<ApiResponse> GetTopPlansAsync()
        {
            var membershipPlans = await _context.PlanMemberships
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Take(3)
                .ToListAsync();

            return ApiResponse.SuccessResponse(membershipPlans);
        }

        public async Task<ApiResponse> GetPlansAsync(string? city, string? category)
        {
            var queryMembership = _context.PlanMemberships.Where(p => p.IsActive).AsQueryable();
            var queryBooking = _context.PlanBookings.Where(p => p.IsActive).AsQueryable();

            if (!string.IsNullOrEmpty(city))
            {
                var cityId = await _context.Cities
                    .Where(c => c.Name == city)
                    .Select(c => c.RecId)
                    .FirstOrDefaultAsync();

                if (cityId != Guid.Empty)
                {
                    var locationIds = await _context.Locations
                        .Where(l => l.FkCity == cityId)
                        .Select(l => l.RecId)
                        .ToListAsync();

                    queryMembership = queryMembership.Where(p => p.FkLocation.HasValue && locationIds.Contains(p.FkLocation.Value));
                    queryBooking = queryBooking.Where(p => p.FkLocation.HasValue && locationIds.Contains(p.FkLocation.Value));
                }
            }

            if (!string.IsNullOrEmpty(category))
            {
                var categoryId = await _context.WorkspaceTypes
                    .Where(t => t.Name == category)
                    .Select(t => t.RecId)
                    .FirstOrDefaultAsync();

                if (categoryId != Guid.Empty)
                {
                    queryMembership = queryMembership.Where(p => p.FkWorkspaceType == categoryId);
                    queryBooking = queryBooking.Where(p => p.FkWorkspaceType == categoryId);
                }
            }

            var memberships = await queryMembership.ToListAsync();
            var bookings = await queryBooking.ToListAsync();

            return ApiResponse.SuccessResponse(new { memberships, bookings });
        }

        public async Task<ApiResponse> GetPlanByIdAsync(Guid id)
        {
            var membership = await _context.PlanMemberships
                .FirstOrDefaultAsync(p => p.RecId == id);

            if (membership != null)
            {
                var cityName = await _context.Locations
                    .Where(l => l.RecId == membership.FkLocation)
                    .Join(_context.Cities, l => l.FkCity, c => c.RecId, (l, c) => c.Name)
                    .FirstOrDefaultAsync();

                var categoryName = await _context.WorkspaceTypes
                    .Where(t => t.RecId == membership.FkWorkspaceType)
                    .Select(t => t.Name)
                    .FirstOrDefaultAsync();

                return ApiResponse.SuccessResponse(new 
                { 
                    plan = membership, 
                    type = "Membership",
                    cityName = cityName ?? "Unknown",
                    categoryName = categoryName ?? "Unknown"
                });
            }

            var booking = await _context.PlanBookings
                .FirstOrDefaultAsync(p => p.RecId == id);

            if (booking != null)
            {
                var cityName = await _context.Locations
                    .Where(l => l.RecId == booking.FkLocation)
                    .Join(_context.Cities, l => l.FkCity, c => c.RecId, (l, c) => c.Name)
                    .FirstOrDefaultAsync();

                var categoryName = await _context.WorkspaceTypes
                    .Where(t => t.RecId == booking.FkWorkspaceType)
                    .Select(t => t.Name)
                    .FirstOrDefaultAsync();

                return ApiResponse.SuccessResponse(new 
                { 
                    plan = booking, 
                    type = "Booking",
                    cityName = cityName ?? "Unknown",
                    categoryName = categoryName ?? "Unknown"
                });
            }

            return ApiResponse.ErrorResponse("Plan not found");
        }
    }
}
