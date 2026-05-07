using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpaceHive360.SuperAdmin.Application.DTOs;
using SpaceHive360.SuperAdmin.Application.Interfaces;

namespace SpaceHive360.Admin.Api.Controllers
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
    }
}
