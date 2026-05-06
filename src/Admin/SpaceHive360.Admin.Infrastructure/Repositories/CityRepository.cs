using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Domain.IRepositories;
using SpaceHive360.Admin.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Infrastructure.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly AdminDbContext _context;

        public CityRepository(AdminDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> GetCompanyIdByUserRecIdAsync(Guid userRecId)
        {
            return await _context.AdminUsers
                .Where(u => u.RecId == userRecId && u.IsActive)
                .Select(u => u.FkCompany)
                .FirstOrDefaultAsync();
        }

        public async Task<List<City>> GetAllAsync(Guid userRecId)
        {
            return await _context.Cities
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<City?> GetByIdAsync(Guid id, Guid userRecId)
        {
            return await _context.Cities
                .FirstOrDefaultAsync(x =>
                    x.RecId == id &&
                    x.IsActive);
        }

        public async Task<Guid> CreateAsync(City entity)
        {
            _context.Cities.Add(entity);
            await _context.SaveChangesAsync();
            return entity.RecId;
        }

        public async Task UpdateAsync(City entity)
        {
            _context.Cities.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(City entity)
        {
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;

            _context.Cities.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
