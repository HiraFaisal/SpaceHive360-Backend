using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Application.Services.Location;

namespace SpaceHive360.Admin.Api.Controllers
{
    [ApiController]
    [Route("api/location")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _service;

        public LocationController(ILocationService service)
        {
            _service = service;
        }

        // 🔹 GET ALL (company-based)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userRecId = GetUserRecId();

            var result = await _service.GetAllAsync(userRecId);

            return Ok(result);
        }

        // 🔹 GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userRecId = GetUserRecId();

            var result = await _service.GetByIdAsync(id, userRecId);

            if (result == null)
                return NotFound(new { Message = "Location not found" });

            return Ok(result);
        }

        // 🔹 CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LocationCreateDTO dto)
        {
            try
            {
                var userRecId = GetUserRecId();

                var id = await _service.CreateAsync(dto, userRecId);

                return Ok(new
                {
                    Message = "Location created successfully",
                    Id = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Failed to create location",
                    Details = ex.Message
                });
            }
        }

        // 🔹 UPDATE
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(Guid id, [FromForm] LocationUpdateDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Invalid request");

                var userRecId = GetUserRecId();

                // 🔥 enforce route id (important)
                model.RecId = id;

                var result = await _service.UpdateAsync(model, userRecId);

                if (!result)
                    return NotFound(new { Message = "Location not found" });

                return Ok(new { Message = "Location updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Something went wrong",
                    error = ex.Message
                });
            }
        }

        // 🔹 DELETE (soft delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userRecId = GetUserRecId();

                var result = await _service.DeleteAsync(id, userRecId);

                if (!result)
                    return NotFound(new { Message = "Location not found" });

                return Ok(new { Message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Failed to delete location",
                    Details = ex.Message
                });
            }
        }

        // 🔐 JWT Helper
        private Guid GetUserRecId()
        {
            var claim = User.FindFirst("recId")?.Value;

            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }
}
