using Microsoft.EntityFrameworkCore;
using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.SuperAdmin.Domain.IRepositories;
using SpaceHive360.SuperAdmin.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.SuperAdmin.Infrastructure.Repositories
{
    public class SuperAdminCompanyRepository : ISuperAdminCompanyRepository
    {
        private readonly SuperAdminDbContext _context;

        public SuperAdminCompanyRepository(SuperAdminDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync(string? status = null)
        {
            var query = _context.Companies.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => c.RegistrationStatus == status);
            }

            return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Company>> GetPendingCompaniesAsync()
        {
            return await _context.Companies
                .Where(c => c.RegistrationStatus == "Pending")
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Company?> GetByIdAsync(Guid id)
        {
            return await _context.Companies.FindAsync(id);
        }

        public async Task UpdateAsync(Company company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }
    }
}
