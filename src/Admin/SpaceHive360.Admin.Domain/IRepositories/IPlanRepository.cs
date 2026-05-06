using SpaceHive360.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Domain.IRepositories
{
    public interface IPlanRepository
    {
        Task<List<Plan>> GetAllAsync(string? search, string? filter, string sortColumn, bool isAscending, int pageNumber, int pageSize);
        Task<List<Plan>> GetByCompanyAsync(Guid companyId);

        Task<Plan?> GetByIdAsync(Guid id);

        Task AddAsync(Plan plan);

        Task UpdateAsync(Plan plan);
        Task<bool> DeleteAsync(Guid id);
    }
}
