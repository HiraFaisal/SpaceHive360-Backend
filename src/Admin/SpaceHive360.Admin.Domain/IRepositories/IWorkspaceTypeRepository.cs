using SpaceHive360.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Domain.IRepositories
{
    public interface IWorkspaceTypeRepository
    {
        Task AddAsync(WorkspaceType entity);
        Task<WorkspaceType?> GetByIdAsync(Guid id);
        Task<IEnumerable<WorkspaceType>> GetAllAsync();
        Task UpdateAsync(WorkspaceType entity);
        Task SaveChangesAsync();

        // Returns the company associated with the given admin user RecId.
        // Synchronous to match current service usage (service calls this without awaiting).
        Company? GetCompanyDetailsByUserRecId(Guid userRecId);
    }
}
