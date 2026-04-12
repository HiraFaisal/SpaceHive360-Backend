using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Application.Services;
using SpaceHive360.Admin.Application.Services.Plans;

namespace SpaceHive360.Admin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlansController : ControllerBase
    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }

        // =========================
        // GET ALL PLANS
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAllPlans(
            [FromQuery] string? search,
            [FromQuery] string? filter,
            [FromQuery] string? sortColumn,
            [FromQuery] bool isAscending = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _planService.GetAllPlansAsync(
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

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanById(Guid id)
        {
            try
            {
                var response = await _planService.GetPlanByIdAsync(id);

                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong", error = ex.Message });
            }
        }

        // =========================
        // CREATE PLAN
        // =========================
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePlan([FromForm] Plans.PlanCreateDto model, [FromForm] IFormFileCollection? images)
        {
            try
            {
                if (model == null) return BadRequest("Invalid request");

                var imageList = images?.ToList() ?? new List<IFormFile>();
                var response = await _planService.CreatePlanAsync(model, imageList);

                return response.Success ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong", error = ex.Message });
            }
        }

        // UPDATE — multipart/form-data
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdatePlan(Guid id, [FromForm] Plans.PlanUpdateDto model, IFormFileCollection? images)
        {
            try
            {
                if (model == null) return BadRequest("Invalid request");

                model.RecId = id;
                var imageList = images?.ToList() ?? new List<IFormFile>();
                var response = await _planService.UpdatePlanAsync(model, imageList);

                return response.Success ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong", error = ex.Message });
            }
        }

        // =========================
        // DELETE PLAN (SOFT DELETE)
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlan(Guid id)
        {
            try
            {
                var response = await _planService.DeletePlanAsync(id);

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