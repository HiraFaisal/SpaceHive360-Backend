using System;
using System.Collections.Generic;
using System.Text;
using static SpaceHive360.Admin.Application.DTOs.Workspace;

namespace SpaceHive360.Admin.Application.Services.WorkspaceType
{
    public interface IWorkspaceTypeService
    {
        Task<Guid> CreateWorkspaceTypeAsync(WorkspaceTypeRequest request, Guid recId);
        Task<Domain.Entities.WorkspaceType?> GetWorkspaceTypeByIdAsync(Guid id);
        Task<IEnumerable<Domain.Entities.WorkspaceType>> GetAllWorkspaceTypesAsync(Guid userRecId);
        Task<bool> UpdateWorkspaceTypeAsync(Guid id, EditWorkspaceTypeRequest dto);
        Task<bool> DeleteWorkspaceTypeAsync(Guid id);
    }
}
