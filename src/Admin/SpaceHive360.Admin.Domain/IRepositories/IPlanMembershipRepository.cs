using SpaceHive360.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Domain.IRepositories
{
    public interface IPlanMembershipRepository
    {
        Task<List<PlanMembership>> GetAllAsync(string? search, string? filter, string sortColumn, bool isAscending, int pageNumber, int pageSize);
        Task<PlanMembership?> GetByIdAsync(Guid id);
        Task AddAsync(PlanMembership planMembership);
        Task UpdateAsync(PlanMembership planMembership);
        Task<bool> DeleteAsync(Guid id);
    }
}
