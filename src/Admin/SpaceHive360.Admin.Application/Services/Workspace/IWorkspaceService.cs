using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SpaceHive360.Admin.Application.DTOs.Workspace;

namespace SpaceHive360.Admin.Application.Services.Workspace
{
    public interface IWorkspaceService
    {
        Task<Guid> CreateWorkspaceAsync(WorkspaceRequest request, Guid userRecId);
        Task<Domain.Entities.Workspace?> GetWorkspaceByIdAsync(Guid id);
        Task<IEnumerable<Domain.Entities.Workspace>> GetAllWorkspacesAsync(Guid userRecId);
        Task<bool> DeleteWorkspaceAsync(Guid id);
    }
}
