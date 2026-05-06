using Microsoft.EntityFrameworkCore;
using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Domain.IRepositories;
using SpaceHive360.Admin.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Infrastructure.Repositories
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly AdminDbContext _context;

        public AdminUserRepository(AdminDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AdminUser user)
        {
            await _context.AdminUsers.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task<AdminUser> GetByEmailAsync(string email)
        {
            return await _context.AdminUsers.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<AdminUser?> GetByIdAsync(Guid id)
        {
            return await _context.AdminUsers.FirstOrDefaultAsync(u => u.RecId == id);
        }


    }
}
