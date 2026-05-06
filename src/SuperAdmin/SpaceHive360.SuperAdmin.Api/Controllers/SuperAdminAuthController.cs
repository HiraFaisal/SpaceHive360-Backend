using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceHive360.SuperAdmin.Application.DTOs;
using SpaceHive360.SuperAdmin.Application.Interfaces;
using System.Threading.Tasks;

namespace SpaceHive360.SuperAdmin.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/superadmin/auth")]
    public class SuperAdminAuthController : ControllerBase
    {
        private readonly ISuperAdminAuthService _authService;

        public SuperAdminAuthController(ISuperAdminAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] SuperAdminLoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
