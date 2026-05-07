using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpaceHive360.SuperAdmin.Application.DTOs;
using SpaceHive360.SuperAdmin.Application.Interfaces;

namespace SpaceHive360.SuperAdmin.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupportController : ControllerBase
    {
        private readonly ISupportService _supportService;

        public SupportController(ISupportService supportService)
        {
            _supportService = supportService;
        }

        [HttpGet("faqs")]
        public async Task<ActionResult<IEnumerable<FaqCategoryDto>>> GetAllFaqs()
        {
            var faqs = await _supportService.GetAllFaqsAsync();
            return Ok(faqs);
        }

        [HttpPost("categories")]
        public async Task<ActionResult<FaqCategoryDto>> CreateCategory([FromBody] CreateFaqCategoryRequest request)
        {
            var category = await _supportService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetAllFaqs), category);
        }

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CreateFaqCategoryRequest request)
        {
            var result = await _supportService.UpdateCategoryAsync(id, request);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var result = await _supportService.DeleteCategoryAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("faqs")]
        public async Task<ActionResult<FaqDto>> CreateFaq([FromBody] CreateFaqRequest request)
        {
            var faq = await _supportService.CreateFaqAsync(request);
            return CreatedAtAction(nameof(GetAllFaqs), faq);
        }

        [HttpPut("faqs/{id}")]
        public async Task<IActionResult> UpdateFaq(Guid id, [FromBody] CreateFaqRequest request)
        {
            var result = await _supportService.UpdateFaqAsync(id, request);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("faqs/{id}")]
        public async Task<IActionResult> DeleteFaq(Guid id)
        {
            var result = await _supportService.DeleteFaqAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
