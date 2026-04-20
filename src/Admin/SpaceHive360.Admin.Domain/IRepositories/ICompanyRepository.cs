using SpaceHive360.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Domain.IRepositories
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByIdAsync(Guid id);
        Task<List<Company>> GetByStatusAsync(string status);
        Task<List<Company>> GetAllAsync();
        Task AddAsync(Company company);
        Task UpdateAsync(Company company);
        Task<bool> ExistsByEmailAsync(string email);
    }
}
