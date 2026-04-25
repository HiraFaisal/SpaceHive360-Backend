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

                Guid userRecId = Guid.Parse(User.Claims.First(c => c.Type == "RecId").Value);
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
                var workspace = await _workspaceService.GetWorkspaceByIdAsync(id);
                if (workspace == null)
                    return NotFound(new { Message = "Workspace not found." });

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
                var recIdClaim = User.Claims.FirstOrDefault(c => c.Type == "recId");

                if (recIdClaim == null || !Guid.TryParse(recIdClaim.Value, out Guid userRecId))
                {
                    return Unauthorized(new { Message = "Invalid or missing user RecId claim." });
                }

                var workspaces = await _workspaceService.GetAllWorkspacesAsync(userRecId);
                return Ok(workspaces);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to retrieve Workspaces.", Details = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _workspaceService.DeleteWorkspaceAsync(id);
                if (!deleted)
                    return NotFound(new { Message = "Workspace not found." });

                return Ok(new { Message = "Workspace deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to delete Workspace.", Details = ex.Message });
            }
        }
    }
}
