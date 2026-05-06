using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Member.Application.Models;
using SpaceHive360.Member.Application.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Api.Controllers
{
    // [Authorize] // Temporarily disabled to debug 500 error
    [ApiController]
    [Route("api/user-preferences")]
    public class UserPreferencesController : ControllerBase
    {
        private readonly IUserPreferenceService _preferenceService;

        public UserPreferencesController(IUserPreferenceService preferenceService)
        {
            _preferenceService = preferenceService;
        }

        private Guid? GetCurrentUserId()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            var token = authHeader.Substring("Bearer ".Length);
            // In our mock system, the token starts with "userId:"
            var parts = token.Split(':');
            if (parts.Length > 0 && Guid.TryParse(parts[0], out Guid userId))
            {
                return userId;
            }

            return null;
        }

        [HttpPost]
        public async Task<IActionResult> SavePreferences([FromBody] UserPreferenceRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse { Success = false, Message = "User ID not found in token." });

            // Validate request
            if (request == null)
                return BadRequest(new ApiResponse { Success = false, Message = "Invalid request body." });

            var result = await _preferenceService.SavePreferencesAsync(userId.Value, request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetPreferences()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse { Success = false, Message = "User ID not found in token." });

            var result = await _preferenceService.GetPreferencesAsync(userId.Value);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
