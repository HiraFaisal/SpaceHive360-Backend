using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Application.Models;
using SpaceHive360.Admin.Application.Services.AdminUsers;
using SpaceHive360.Admin.Application.Services.PlanBookings;
using SpaceHive360.Admin.Application.Services.PlanMemberships;
using SpaceHive360.Admin.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlansController : ControllerBase
    {
        private readonly IPlanMembershipService _planMembershipService;
        private readonly IPlanBookingService _planBookingService;
        private readonly IAdminUserService _adminUserService;
        private readonly AdminDbContext _context;

        public PlansController(
            IPlanMembershipService planMembershipService, 
            IPlanBookingService planBookingService, 
            IAdminUserService adminUserService,
            AdminDbContext context)
        {
            _planMembershipService = planMembershipService;
            _planBookingService = planBookingService;
            _adminUserService = adminUserService;
            _context = context;
        }

        [HttpGet("company")]
        public async Task<IActionResult> GetPlansByCompany()
        {
            try
            {
                if (!TryGetUserId(out Guid userId)) return Unauthorized();

                var user = await _adminUserService.GetUserByIdAsync(userId);
                if (user == null) return NotFound("Admin user not found");

                // 1. Fetch Membership Plans with Joins
                var memberships = await (from m in _context.PlanMemberships
                                   join wt in _context.WorkspaceTypes on m.FkWorkspaceType equals wt.RecId into wtJoin from wt in wtJoin.DefaultIfEmpty()
                                   join w in _context.Workspaces on m.FkWorkspace equals w.RecId into wJoin from w in wJoin.DefaultIfEmpty()
                                   join l in _context.CompanyLocations on m.FkLocation equals l.RecId into lJoin from l in lJoin.DefaultIfEmpty()
                                   join c in _context.Cities on l.FkCity equals c.RecId into cJoin from c in cJoin.DefaultIfEmpty()
                                   where m.FkCompany == user.FkCompany && m.IsActive && 
                                         !(m.IsAiUpdated && m.UpdatedAt > DateTime.UtcNow.AddDays(-1))
                                   select new {
                                       recId = m.RecId,
                                       name = m.Name,
                                       price = m.Price,
                                       description = m.Description ?? "Membership Plan",
                                       type = "Membership",
                                       workspaceTypeName = wt != null ? wt.Name : "N/A",
                                       workspaceName = w != null ? w.Name : "N/A",
                                       locationName = l != null ? l.Name : "N/A",
                                       cityName = c != null ? c.Name : "Karachi"
                                   }).ToListAsync();

                // 2. Fetch Booking Plans with Joins
                var bookings = await (from b in _context.PlanBookings
                                join wt in _context.WorkspaceTypes on b.FkWorkspaceType equals wt.RecId into wtJoin from wt in wtJoin.DefaultIfEmpty()
                                join w in _context.Workspaces on b.FkWorkspace equals w.RecId into wJoin from w in wJoin.DefaultIfEmpty()
                                join l in _context.CompanyLocations on b.FkLocation equals l.RecId into lJoin from l in lJoin.DefaultIfEmpty()
                                join c in _context.Cities on l.FkCity equals c.RecId into cJoin from c in cJoin.DefaultIfEmpty()
                                where b.FkCompany == user.FkCompany && b.IsActive && 
                                      !(b.IsAiUpdated && b.UpdatedAt > DateTime.UtcNow.AddDays(-1))
                                select new {
                                    recId = b.RecId,
                                    name = b.Name,
                                    price = b.Price,
                                    description = b.Description ?? "Booking Plan",
                                    type = "Booking",
                                    workspaceTypeName = wt != null ? wt.Name : "N/A",
                                    workspaceName = w != null ? w.Name : "N/A",
                                    locationName = l != null ? l.Name : "N/A",
                                    cityName = c != null ? c.Name : "Karachi"
                                }).ToListAsync();

                var combinedPlans = new List<object>();
                combinedPlans.AddRange(memberships);
                combinedPlans.AddRange(bookings);

                return Ok(ApiResponse.SuccessResponse(combinedPlans, "All company plans enriched with full metadata fetched successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanById(Guid id)
        {
            try
            {
                if (!TryGetUserId(out Guid userId)) return Unauthorized();

                // Try Membership first
                var membershipResponse = await _planMembershipService.GetPlanMembershipByIdAsync(id);
                if (membershipResponse.Success && membershipResponse.Data != null)
                {
                    return Ok(membershipResponse);
                }

                // Try Booking next
                var bookingResponse = await _planBookingService.GetPlanBookingByIdAsync(id, userId);
                if (bookingResponse.Success && bookingResponse.Data != null)
                {
                    return Ok(bookingResponse);
                }

                return NotFound(ApiResponse.ErrorResponse("Plan not found", 404));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong", error = ex.Message });
            }
        }

        [HttpPost("update-price")]
        public async Task<IActionResult> UpdatePlanPrice([FromBody] Plans.UpdatePriceDto request)
        {
            try
            {
                if (!TryGetUserId(out Guid userId)) return Unauthorized();

                // 1. Try to find and update in PlanMemberships
                var membership = await _context.PlanMemberships
                    .FirstOrDefaultAsync(m => m.RecId == request.PlanId);

                if (membership != null)
                {
                    membership.Price = request.NewPrice;
                    membership.IsAiUpdated = true;
                    membership.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return Ok(ApiResponse.SuccessResponse(true, "Membership plan price updated via AI successfully"));
                }

                // 2. Try to find and update in PlanBookings
                var booking = await _context.PlanBookings
                    .FirstOrDefaultAsync(b => b.RecId == request.PlanId);

                if (booking != null)
                {
                    booking.Price = request.NewPrice;
                    booking.IsAiUpdated = true;
                    booking.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return Ok(ApiResponse.SuccessResponse(true, "Booking plan price updated via AI successfully"));
                }

                return NotFound(ApiResponse.ErrorResponse("Plan not found in either membership or booking tables", 404));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong during price update", error = ex.Message });
            }
        }

        private bool TryGetUserId(out Guid userId)
        {
            userId = Guid.Empty;
            var claim = User.Claims.FirstOrDefault(c => c.Type.Equals("recId", StringComparison.OrdinalIgnoreCase));
            if (claim == null || !Guid.TryParse(claim.Value, out userId))
                return false;
            return true;
        }
    }
}