using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Application.Services;
using SpaceHive360.Admin.Application.Services.WorkspaceType;
using SpaceHive360.Admin.Domain.Entities;
using static SpaceHive360.Admin.Application.DTOs.Workspace;

namespace SpaceHive360.Admin.Api.Controllers
{
    [Route("api/workspace-types")]
    [ApiController]
    public class WorkspaceTypeController : ControllerBase
    {
        private readonly IWorkspaceTypeService _workspaceTypeService;

        public WorkspaceTypeController(IWorkspaceTypeService workspaceTypeService)
        {
            _workspaceTypeService = workspaceTypeService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WorkspaceTypeRequest workspaceType)
        {
            try
            {
                var recIdClaim = User.Claims.FirstOrDefault(c => c.Type == "recId");

                if (recIdClaim == null || !Guid.TryParse(recIdClaim.Value, out Guid userRecId))
                {
                    return Unauthorized(new { Message = "Invalid or missing user RecId claim." });
                }

                var id = await _workspaceTypeService.CreateWorkspaceTypeAsync(workspaceType, userRecId);
                return Ok(new { Message = "Workspace Type created successfully.", Id = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to create Workspace Type.", Details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var type = await _workspaceTypeService.GetWorkspaceTypeByIdAsync(id);
                if (type == null)
                    return NotFound(new { Message = "Workspace Type not found." });

                return Ok(type);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to retrieve Workspace Type.", Details = ex.Message });
            }
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

                var types = await _workspaceTypeService.GetAllWorkspaceTypesAsync(userRecId);
                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to retrieve Workspace Types.", Details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EditWorkspaceTypeRequest dto)
        {
            try
            {
                var updated = await _workspaceTypeService.UpdateWorkspaceTypeAsync(id, dto);
                if (!updated)
                    return NotFound(new { Message = "Workspace Type not found." });

                return Ok(new { Message = "Workspace Type updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to update Workspace Type.", Details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _workspaceTypeService.DeleteWorkspaceTypeAsync(id);
                if (!deleted)
                    return NotFound(new { Message = "Workspace Type not found." });

                return Ok(new { Message = "Workspace Type deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to delete Workspace Type.", Details = ex.Message });
            }
        }
    }
}
