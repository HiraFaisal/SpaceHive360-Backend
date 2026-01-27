using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Admin.Application.Services;
using SpaceHive360.Admin.Application.Services.AdminUsers;
using SpaceHive360.Admin.Domain.Entities;

[ApiController]
[Route("api/admin-users")]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _service;

    public AdminUsersController(IAdminUserService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(AdminUser user)
    {
        await _service.CreateAsync(user);
        return Ok("Admin user created");
    }
}
