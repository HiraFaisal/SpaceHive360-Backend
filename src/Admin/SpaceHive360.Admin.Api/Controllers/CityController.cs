using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Admin.Application.Services.Cities;
using System;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Api.Controllers
{
    [ApiController]
    [Route("api/city")]
    public class CityController : ControllerBase
    {
        private readonly ICityService _service;

        public CityController(ICityService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userRecId = GetUserRecId();
            var result = await _service.GetAllAsync(userRecId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userRecId = GetUserRecId();
            var result = await _service.GetByIdAsync(id, userRecId);
            if (result == null)
                return NotFound(new { Message = "City not found" });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CityCreateDTO dto)
        {
            var userRecId = GetUserRecId();
            var id = await _service.CreateAsync(dto, userRecId);
            return Ok(new { Message = "City created successfully", Id = id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CityUpdateDTO dto)
        {
            var userRecId = GetUserRecId();
            dto.RecId = id;
            var result = await _service.UpdateAsync(dto, userRecId);
            if (!result)
                return NotFound(new { Message = "City not found" });
            return Ok(new { Message = "City updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userRecId = GetUserRecId();
            var result = await _service.DeleteAsync(id, userRecId);
            if (!result)
                return NotFound(new { Message = "City not found" });
            return Ok(new { Message = "City deleted successfully" });
        }

        private Guid GetUserRecId()
        {
            var claim = User.FindFirst("recId")?.Value;
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }
}
