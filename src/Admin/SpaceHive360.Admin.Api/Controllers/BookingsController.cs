using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Admin.Application.Services.Bookings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (!TryGetUserId(out Guid adminId)) return Unauthorized();
            var res = await _bookingService.GetBookingsAsync(adminId, search, page, pageSize);
            return Ok(res);
        }

        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (!TryGetUserId(out Guid adminId)) return Unauthorized();
            var res = await _bookingService.GetPaymentHistoryAsync(adminId, page, pageSize);
            return Ok(res);
        }

        [HttpGet("calendar")]
        public async Task<IActionResult> GetCalendar()
        {
            if (!TryGetUserId(out Guid adminId)) return Unauthorized();
            var res = await _bookingService.GetCalendarBookingsAsync(adminId);
            return Ok(res);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            if (!TryGetUserId(out Guid adminId)) return Unauthorized();
            var res = await _bookingService.GetDashboardStatsAsync(adminId);
            return Ok(res);
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
