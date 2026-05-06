using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Member.Application.Models;
using SpaceHive360.Member.Application.Services;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService _membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseMembership([FromForm] MembershipPurchaseRequest request)
        {
            if (request == null) return BadRequest("Invalid request");

            var response = await _membershipService.PurchaseMembershipAsync(request);
            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("purchase-booking")]
        public async Task<IActionResult> PurchaseBooking([FromForm] BookingPurchaseRequest request)
        {
            if (request == null) return BadRequest("Invalid request");

            var response = await _membershipService.PurchaseBookingAsync(request);
            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyMemberships()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(ApiResponse.ErrorResponse("User ID not found in token."));

            var response = await _membershipService.GetMyMembershipsAsync(userId.Value);
            return Ok(response);
        }

        private Guid? GetCurrentUserId()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            var token = authHeader.Substring("Bearer ".Length);
            var parts = token.Split(':');
            if (parts.Length > 0 && Guid.TryParse(parts[0], out Guid userId))
            {
                return userId;
            }

            return null;
        }
    }
}
