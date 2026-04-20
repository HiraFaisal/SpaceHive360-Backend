using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Application.Services.PlanBookings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanBookingsController : ControllerBase
    {
        private readonly IPlanBookingService _planBookingService;

        public PlanBookingsController(IPlanBookingService planBookingService)
        {
            _planBookingService = planBookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlanBookings(
            [FromQuery] string? search,
            [FromQuery] string? filter,
            [FromQuery] string? sortColumn,
            [FromQuery] bool isAscending = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _planBookingService.GetAllPlanBookingsAsync(
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
        public async Task<IActionResult> GetPlanBookingById(Guid id)
        {
            try
            {
                var response = await _planBookingService.GetPlanBookingByIdAsync(id);

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
        public async Task<IActionResult> CreatePlanBooking([FromForm] PlanBookings.PlanBookingCreateDto model)
        {
            try
            {
                if (model == null) return BadRequest("Invalid request");

                var imageList = model.Images?.ToList() ?? new System.Collections.Generic.List<IFormFile>();
                var response = await _planBookingService.CreatePlanBookingAsync(model, imageList);

                return response.Success ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdatePlanBooking(Guid id, [FromForm] PlanBookings.PlanBookingUpdateDto model)
        {
            try
            {
                if (model == null) return BadRequest("Invalid request");

                model.RecId = id;
                var imageList = model.Images?.ToList() ?? new System.Collections.Generic.List<IFormFile>();
                var response = await _planBookingService.UpdatePlanBookingAsync(model, imageList);

                return response.Success ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlanBooking(Guid id)
        {
            try
            {
                var response = await _planBookingService.DeletePlanBookingAsync(id);

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
