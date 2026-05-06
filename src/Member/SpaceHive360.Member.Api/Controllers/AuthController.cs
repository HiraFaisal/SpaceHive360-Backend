using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Member.Application.Models;
using SpaceHive360.Member.Application.Services;
using System;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _authService.RegisterAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetProfile([FromQuery] Guid userId)
        {
            var response = await _authService.GetProfileAsync(userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
