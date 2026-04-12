using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Application.Services;
using SpaceHive360.Admin.Application.Services.PaymentTerms;

namespace SpaceHive360.Admin.Api.Controllers
{
    [Route("api/payment-terms")]
    [ApiController]
    public class PaymentTermsController : ControllerBase
    {
        private readonly IPaymentTermsService _service;
        public PaymentTermsController(IPaymentTermsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var recIdClaim = User.Claims.FirstOrDefault(c => c.Type == "recId");

                if (recIdClaim == null || !Guid.TryParse(recIdClaim.Value, out Guid userRecId))
                {
                    return Unauthorized(new { Message = "Invalid or missing user RecId claim." });
                }

                var result = await _service.GetAllPaymentTermsAsync(userRecId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to fetch Payment Terms.", Details = ex.Message });
            }
        }

        // 🔹 GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userRecId = GetUserRecId();
            var result = await _service.GetByIdAsync(id, userRecId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // 🔹 CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentTermsCreateDTO dto)
        {
            var userRecId = GetUserRecId();

            var id = await _service.CreateAsync(dto, userRecId);

            return Ok(new
            {
                Message = "Payment Term created successfully",
                Id = id
            });
        }

        // 🔹 UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] PaymentTermsUpdateDTO dto)
        {
            try
            {
                var userRecId = GetUserRecId();

                var result = await _service.UpdateAsync(dto, userRecId);

                if (!result)
                    return NotFound(new { Message = "Payment Term not found." });

                return Ok(new { Message = "Payment Term updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Failed to update Payment Term.",
                    Details = ex.Message
                });
            }
        }

        // 🔹 DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userRecId = GetUserRecId();

            var result = await _service.DeleteAsync(id, userRecId);

            if (!result)
                return NotFound();

            return Ok(new { Message = "Deleted successfully" });
        }

        // 🔐 JWT helper
        private Guid GetUserRecId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "recId");

            if (claim == null || !Guid.TryParse(claim.Value, out Guid userRecId))
            {
                throw new UnauthorizedAccessException("Invalid token");
            }

            return userRecId;
        }
    }
}
