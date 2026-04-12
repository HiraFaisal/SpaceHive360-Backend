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
    public class WorkspaceTypeRepository : IWorkspaceTypeRepository
    {
        private readonly AdminDbContext _context;
        private readonly DbSet<WorkspaceType> _dbSet;

        public WorkspaceTypeRepository(AdminDbContext context)
        {
            _context = context;
            _dbSet = _context.WorkspaceTypes;
        }

        public async Task AddAsync(WorkspaceType entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<WorkspaceType?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<WorkspaceType>> GetAllAsync()
        {
            return await _dbSet
        .Where(x => x.IsActive)
        .ToListAsync();
        }

        public Task UpdateAsync(WorkspaceType entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Company? GetCompanyDetailsByUserRecId(Guid userRecId)
        {
            // Find the admin user by RecId, then return the company that matches the user's FkCompany.
            var user = _context.AdminUsers.FirstOrDefault(u => u.RecId == userRecId);
            if (user == null) return null;

            return _context.Companies.FirstOrDefault(c => c.RecId == user.FkCompany);
        }
    }
}
