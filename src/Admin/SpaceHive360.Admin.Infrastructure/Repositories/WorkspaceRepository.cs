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
    public class WorkspaceRepository : IWorkspaceRepository
    {
        private readonly AdminDbContext _context;

        public WorkspaceRepository(AdminDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Workspace entity)
        {
            await _context.Workspaces.AddAsync(entity);
        }

        public async Task<Workspace?> GetByIdAsync(Guid id)
        {
            return await _context.Workspaces.FindAsync(id);
        }

        public async Task<IEnumerable<Workspace>> GetAllWorkspaceAsync(Guid companyId)
        {
            return await _context.Workspaces
                .Where(w => w.FkCompany == companyId && w.IsActive)
                .ToListAsync();
        }

        public Task UpdateAsync(Workspace entity)
        {
            _context.Workspaces.Update(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Company? GetCompanyDetailsByUserRecId(Guid userRecId)
        {
            var user = _context.AdminUsers.FirstOrDefault(u => u.RecId == userRecId);
            if (user == null) return null;
            return _context.Companies.FirstOrDefault(c => c.RecId == user.FkCompany);
        }

        public async Task<string> GenerateUniqueWorkspaceCodeAsync()
        {
            const int maxAttempts = 20;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Generate a 4-digit number padded with leading zeros: 0000-9999
                int num = Random.Shared.Next(0, 10000);
                string code = $"WS-{num:0000}";

                bool exists = await _context.Workspaces.AnyAsync(w => w.UniqueCode == code);
                if (!exists)
                    return code;
            }

            throw new InvalidOperationException("Unable to generate a unique workspace code after multiple attempts.");
        }
    }
}