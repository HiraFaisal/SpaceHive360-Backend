using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Application.Services;
using SpaceHive360.Admin.Application.Services.Workspace;
using static SpaceHive360.Admin.Application.DTOs.Workspace;

namespace SpaceHive360.Admin.Api.Controllers
{
    [Route("api/workspaces")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        private readonly IWorkspaceService _workspaceService;

        public WorkspaceController(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WorkspaceRequest dto)
        {
            try
            {
                if (!TryGetUserId(out Guid userRecId)) return Unauthorized();
                
                var id = await _workspaceService.CreateWorkspaceAsync(dto, userRecId);
                return Ok(new { Message = "Workspace created successfully.", Id = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to create Workspace.", Details = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                if (!TryGetUserId(out Guid userRecId)) return Unauthorized();

                var workspace = await _workspaceService.GetWorkspaceByIdAsync(id, userRecId);
                if (workspace == null)
                    return NotFound(new { Message = "Workspace not found or unauthorized." });

                return Ok(workspace);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to retrieve Workspace.", Details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (!TryGetUserId(out Guid userRecId)) return Unauthorized();

                var workspaces = await _workspaceService.GetAllWorkspacesAsync(userRecId);
                return Ok(workspaces);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to retrieve Workspaces.", Details = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] WorkspaceRequest dto)
        {
            try
            {
                if (!TryGetUserId(out Guid userRecId)) return Unauthorized();

                var updated = await _workspaceService.UpdateWorkspaceAsync(id, dto, userRecId);
                if (!updated)
                    return NotFound(new { Message = "Workspace not found or unauthorized." });

                return Ok(new { Message = "Workspace updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to update Workspace.", Details = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (!TryGetUserId(out Guid userRecId)) return Unauthorized();

                var deleted = await _workspaceService.DeleteWorkspaceAsync(id, userRecId);
                if (!deleted)
                    return NotFound(new { Message = "Workspace not found or unauthorized." });

                return Ok(new { Message = "Workspace deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to delete Workspace.", Details = ex.Message });
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                if (!TryGetUserId(out Guid userRecId)) return Unauthorized();

                var stats = await _workspaceService.GetWorkspaceStatsAsync(userRecId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to retrieve Workspace stats.", Details = ex.Message });
            }
        }

        private bool TryGetUserId(out Guid userId)
        {
            userId = Guid.Empty;
            var claim = User.Claims.FirstOrDefault(c => c.Type.Equals("recId", StringComparison.OrdinalIgnoreCase));
            if (claim == null || !Guid.TryParse(claim.Value, out userId))
                return false;
            return true;
        }
    }
}
