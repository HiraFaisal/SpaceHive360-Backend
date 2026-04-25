using SpaceHive360.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SpaceHive360.Admin.Domain.Models;

namespace SpaceHive360.Admin.Domain.IRepositories
{
    public interface IPlanMembershipRepository
    {
        Task<List<PlanMembership>> GetAllAsync(Guid? companyId, string? search, string? filter, string sortColumn, bool isAscending, int pageNumber, int pageSize);
        Task<PlanMembership?> GetByIdAsync(Guid id);
        Task AddAsync(PlanMembership planMembership);
        Task UpdateAsync(PlanMembership planMembership);
        Task<bool> DeleteAsync(Guid id);
        Task<PlanMembershipStats> GetStatsAsync(Guid? companyId);
    }
}
