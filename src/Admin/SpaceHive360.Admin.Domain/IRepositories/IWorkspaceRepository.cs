using SpaceHive360.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Domain.IRepositories
{
    public interface IWorkspaceRepository
    {
        Task AddAsync(Workspace entity);
        Task<Workspace?> GetByIdAsync(Guid id);
        Task<IEnumerable<Workspace>> GetAllWorkspaceAsync(Guid companyId);
        Task UpdateAsync(Workspace entity);
        Task SaveChangesAsync();

        // Returns the company associated with the given admin user RecId.
        // Kept synchronous to match current service usage.
        Company? GetCompanyDetailsByUserRecId(Guid userRecId);

        // Generates a unique workspace code like "WS-1234".
        // Ensures the generated code does not already exist in the database.
        Task<string> GenerateUniqueWorkspaceCodeAsync();
    }
}