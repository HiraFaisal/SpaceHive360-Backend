using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceHive360.SuperAdmin.Application.DTOs;
using SpaceHive360.SuperAdmin.Application.Interfaces;
using System.Threading.Tasks;

namespace SpaceHive360.SuperAdmin.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/superadmin/setup")]
    public class SuperAdminSetupController : ControllerBase
    {
        private readonly ISuperAdminUserService _userService;

        public SuperAdminSetupController(ISuperAdminUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateSuperAdminRequest request)
        {
            try
            {
                var response = await _userService.CreateAsync(request);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
