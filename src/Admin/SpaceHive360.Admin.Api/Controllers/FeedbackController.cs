using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Application.IRepositories;
using SpaceHive360.Admin.Application.Services.Feedback;

namespace SpaceHive360.Admin.Api.Controllers
{
    [Route("api/feedback")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        // 🔹 GET: api/feedback?recId=optional
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? recId)
        {
            try
            {
                var userRecId = GetUserRecId();

                var result = await _feedbackService.GetFeedbacksAsync(recId, userRecId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Failed to retrieve Feedbacks.",  
                    Details = ex.Message
                });
            }
        }

        [HttpGet("location-summary/{locationId}")]
        public async Task<IActionResult> GetLocationSummary(Guid locationId)
        {
            try
            {
                var userRecId = GetUserRecId();
                var result = await _feedbackService.GetLocationSentimentSummaryAsync(locationId, userRecId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Failed to retrieve Location Sentiment Summary.",
                    Details = ex.Message
                });
            }
        }

        // 🔐 JWT Helper
        private Guid GetUserRecId()
        {
            var claim = User.FindFirst("recId")?.Value;

            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }
}