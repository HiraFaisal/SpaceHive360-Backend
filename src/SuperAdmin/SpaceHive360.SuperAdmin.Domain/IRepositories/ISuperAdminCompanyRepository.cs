using SpaceHive360.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceHive360.SuperAdmin.Domain.IRepositories
{
    public interface ISuperAdminCompanyRepository
    {
        Task<IEnumerable<Company>> GetAllCompaniesAsync(string? status = null);
        Task<IEnumerable<Company>> GetPendingCompaniesAsync();
        Task<Company?> GetByIdAsync(Guid id);
        Task UpdateAsync(Company company);
    }
}
