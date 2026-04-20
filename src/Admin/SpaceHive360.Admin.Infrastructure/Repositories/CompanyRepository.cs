using Microsoft.EntityFrameworkCore;
using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Domain.IRepositories;
using SpaceHive360.Admin.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly AdminDbContext _context;

        public CompanyRepository(AdminDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Company?> GetByIdAsync(Guid id)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.RecId == id);
        }

        public async Task<List<Company>> GetByStatusAsync(string status)
        {
            return await _context.Companies.Where(c => c.RegistrationStatus == status).ToListAsync();
        }

        public async Task<List<Company>> GetAllAsync()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task AddAsync(Company company)
        {
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Company company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Companies.AnyAsync(c => c.Email.ToLower() == email.ToLower());
        }
    }
}
