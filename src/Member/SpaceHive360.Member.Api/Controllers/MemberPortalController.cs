using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Member.Application.Models;
using SpaceHive360.Member.Application.Services;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberPortalController : ControllerBase
    {
        private readonly IMemberPortalService _memberService;

        public MemberPortalController(IMemberPortalService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet("cities")]
        public async Task<IActionResult> GetCities()
        {
            var response = await _memberService.GetCitiesAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var response = await _memberService.GetCategoriesAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("plans/top")]
        public async Task<IActionResult> GetTopPlans()
        {
            var response = await _memberService.GetTopPlansAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("plans")]
        public async Task<IActionResult> GetPlans(
            [FromQuery] string? city,
            [FromQuery] string? category)
        {
            var response = await _memberService.GetPlansAsync(city, category);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("plans/{id}")]
        public async Task<IActionResult> GetPlanById(Guid id)
        {
            var response = await _memberService.GetPlanByIdAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("locations")]
        public async Task<IActionResult> GetLocations()
        {
            var response = await _memberService.GetLocationsAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var response = await _memberService.GetStatsAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
