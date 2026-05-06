using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Application.Services.Companies;
using System;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        // 🔹 REGISTER (Public)
        // This allows a new company admin to register their company.
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CompanyRegistrationDto dto)
        {
            if (dto == null) return BadRequest("Invalid request.");

            var response = await _companyService.RegisterCompanyAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // 🔹 CHECK STATUS (Public - Anonymous)
        // This allows the company admin to check their registration status using their email
        // without needing to be logged in with a JWT.
        [HttpGet("check-status")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckStatus([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required to check status.");

            var response = await _companyService.GetCompanyStatusAsync(email);
            return response.Success ? Ok(response) : NotFound(response);
        }

        // 🔹 GET BY ID (Internal/Admin use if needed later)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _companyService.GetCompanyByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}
