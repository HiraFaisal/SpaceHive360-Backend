using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Application.Models;
using SpaceHive360.Admin.Application.Services.AdminUsers;
using SpaceHive360.Admin.Application.Services.PlanBookings;
using SpaceHive360.Admin.Application.Services.PlanMemberships;
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

        public PlansController(
            IPlanMembershipService planMembershipService, 
            IPlanBookingService planBookingService, 
            IAdminUserService adminUserService)
        {
            _planMembershipService = planMembershipService;
            _planBookingService = planBookingService;
            _adminUserService = adminUserService;
        }

        [HttpGet("company")]
        public async Task<IActionResult> GetPlansByCompany()
        {
            try
            {
                if (!TryGetUserId(out Guid userId)) return Unauthorized();

                var user = await _adminUserService.GetUserByIdAsync(userId);
                if (user == null) return NotFound("Admin user not found");

                // 1. Fetch Membership Plans
                var membershipResponse = await _planMembershipService.GetAllPlanMembershipsAsync(user.FkCompany, null, null, "name", true, 1, 100);
                
                // 2. Fetch Booking Plans
                var bookingResponse = await _planBookingService.GetAllPlanBookingsAsync(userId, null, null, "name", true, 1, 100);

                var combinedPlans = new List<object>();

                if (membershipResponse.Success && membershipResponse.Data != null)
                {
                    if (membershipResponse.Data is IEnumerable<PlanMemberships.PlanMembershipDto> memberships)
                    {
                        combinedPlans.AddRange(memberships.Select(m => new {
                            recId = m.RecId,
                            name = m.Name,
                            price = m.Price,
                            description = m.Description ?? "Membership Plan",
                            type = "Membership"
                        }));
                    }
                }

                if (bookingResponse.Success && bookingResponse.Data != null)
                {
                    if (bookingResponse.Data is IEnumerable<PlanBookings.PlanBookingDto> bookings)
                    {
                        combinedPlans.AddRange(bookings.Select(b => new {
                            recId = b.RecId,
                            name = b.Name,
                            price = b.Price,
                            description = b.Description ?? "Booking Plan",
                            type = "Booking"
                        }));
                    }
                }

                return Ok(ApiResponse.SuccessResponse(combinedPlans, "All company plans (Memberships & Bookings) fetched successfully"));
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