using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Member.Application.Models;
using SpaceHive360.Member.Application.Services;
using System;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitFeedback([FromBody] FeedbackRequest request)
        {
            var result = await _feedbackService.SubmitFeedbackAsync(request);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetFeedbackForBooking(Guid bookingId)
        {
            var result = await _feedbackService.GetFeedbackForBookingAsync(bookingId);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("membership/{membershipId}")]
        public async Task<IActionResult> GetFeedbackForMembership(Guid membershipId)
        {
            var result = await _feedbackService.GetFeedbackForMembershipAsync(membershipId);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentFeedback([FromQuery] int count = 5)
        {
            var result = await _feedbackService.GetRecentFeedbackAsync(count);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
