using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Application.Services.PlanMemberships;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanMembershipsController : ControllerBase
    {
        private readonly IPlanMembershipService _planMembershipService;

        public PlanMembershipsController(IPlanMembershipService planMembershipService)
        {
            _planMembershipService = planMembershipService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlanMemberships(
            [FromQuery] string? search,
            [FromQuery] string? filter,
            [FromQuery] string? sortColumn,
            [FromQuery] bool isAscending = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var companyIdClaim = User.FindFirst("companyId")?.Value;
                Guid? companyId = !string.IsNullOrEmpty(companyIdClaim) ? Guid.Parse(companyIdClaim) : null;

                var response = await _planMembershipService.GetAllPlanMembershipsAsync(
                    companyId,
                    search, 
                    filter, 
                    sortColumn ?? "name", 
                    isAscending, 
                    pageNumber, 
                    pageSize
                );

                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanMembershipById(Guid id)
        {
            try
            {
                var response = await _planMembershipService.GetPlanMembershipByIdAsync(id);

                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong", error = ex.Message });
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePlanMembership([FromForm] PlanMemberships.PlanMembershipCreateDto model)
        {
            try
            {
                if (model == null) return BadRequest("Invalid request");

                var imageList = model.Images?.ToList() ?? new System.Collections.Generic.List<IFormFile>();
                var response = await _planMembershipService.CreatePlanMembershipAsync(model, imageList);

                return response.Success ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdatePlanMembership(Guid id, [FromForm] PlanMemberships.PlanMembershipUpdateDto model)
        {
            try
            {
                if (model == null) return BadRequest("Invalid request");

                model.RecId = id;
                var imageList = model.Images?.ToList() ?? new System.Collections.Generic.List<IFormFile>();
                var response = await _planMembershipService.UpdatePlanMembershipAsync(model, imageList);

                return response.Success ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlanMembership(Guid id)
        {
            try
            {
                var response = await _planMembershipService.DeletePlanMembershipAsync(id);

                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong", error = ex.Message });
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var companyIdClaim = User.FindFirst("companyId")?.Value;
                Guid? companyId = !string.IsNullOrEmpty(companyIdClaim) ? Guid.Parse(companyIdClaim) : null;

                var response = await _planMembershipService.GetPlanMembershipStatsAsync(companyId);

                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong", error = ex.Message });
            }
        }
    }
}
