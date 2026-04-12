using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Domain.IRepositories;
using SpaceHive360.Admin.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Infrastructure.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly AdminDbContext _context;

        public LocationRepository(AdminDbContext context)
        {
            _context = context;
        }

        // 🔹 Get Company Id (SaaS core)
        public async Task<Guid> GetCompanyIdByUserRecIdAsync(Guid userRecId)
        {
            return await _context.AdminUsers
                .Where(u => u.RecId == userRecId && u.IsActive)
                .Select(u => u.FkCompany)
                .FirstOrDefaultAsync();
        }

        // 🔹 GET ALL (SaaS FILTERED)
        public async Task<List<Location>> GetAllAsync(Guid userRecId)
        {
            var companyId = await GetCompanyIdByUserRecIdAsync(userRecId);

            return await _context.CompanyLocations
                .Where(x => x.FkCompany == companyId && x.IsActive)
                .ToListAsync();
        }

        // 🔹 GET BY ID (SaaS SAFE)
        public async Task<Location?> GetByIdAsync(Guid id, Guid userRecId)
        {
            var companyId = await GetCompanyIdByUserRecIdAsync(userRecId);

            return await _context.CompanyLocations
                .FirstOrDefaultAsync(x =>
                    x.RecId == id &&
                    x.FkCompany == companyId &&
                    x.IsActive);
        }

        // 🔹 CREATE
        public async Task<Guid> CreateAsync(Location entity)
        {
            _context.CompanyLocations.Add(entity);
            await _context.SaveChangesAsync();
            return entity.RecId;
        }

        // 🔹 UPDATE
        public async Task UpdateAsync(Location entity)
        {
            _context.CompanyLocations.Update(entity);
            await _context.SaveChangesAsync();
        }

        // 🔹 SOFT DELETE
        public async Task DeleteAsync(Location entity)
        {
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;

            _context.CompanyLocations.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
