using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceHive360.SuperAdmin.Application.DTOs;
using SpaceHive360.SuperAdmin.Application.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpaceHive360.SuperAdmin.Api.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [ApiController]
    [Route("api/superadmin/companies")]
    public class SuperAdminCompanyController : ControllerBase
    {
        private readonly ISuperAdminCompanyService _companyService;

        public SuperAdminCompanyController(ISuperAdminCompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var companies = await _companyService.GetAllCompaniesAsync(status);
            return Ok(companies);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var companies = await _companyService.GetPendingCompaniesAsync();
            return Ok(companies);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id, [FromBody] CompanyActionRequestDto dto)
        {
            var superAdminId = GetCurrentUserId();
            var result = await _companyService.ApproveCompanyAsync(id, dto.Remarks, superAdminId);
            return result ? Ok(new { message = "Company approved successfully." }) : NotFound("Company not found.");
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] CompanyActionRequestDto dto)
        {
            var superAdminId = GetCurrentUserId();
            var result = await _companyService.RejectCompanyAsync(id, dto.Remarks, superAdminId);
            return result ? Ok(new { message = "Company rejected successfully." }) : NotFound("Company not found.");
        }

        [HttpPost("{id}/review")]
        public async Task<IActionResult> Review(Guid id, [FromBody] CompanyActionRequestDto dto)
        {
            var superAdminId = GetCurrentUserId();
            var result = await _companyService.ReviewCompanyAsync(id, dto.Remarks, superAdminId);
            return result ? Ok(new { message = "Company marked as under review." }) : NotFound("Company not found.");
        }

        private Guid GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
        }
    }
}
