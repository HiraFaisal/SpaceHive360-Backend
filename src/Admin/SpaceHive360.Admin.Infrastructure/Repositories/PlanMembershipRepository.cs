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
    public class PlanMembershipRepository : IPlanMembershipRepository
    {
        private readonly AdminDbContext _context;

        public PlanMembershipRepository(AdminDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<PlanMembership>> GetAllAsync(
            string? search,
            string? filter,
            string sortColumn,
            bool isAscending,
            int pageNumber,
            int pageSize)
        {
            try
            {
                var query = _context.PlanMemberships.Where(pm => pm.IsActive).AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(pm => pm.Name != null && pm.Name.ToLower().Contains(search.ToLower()));
                }

                // Simple ordering
                if (!string.IsNullOrWhiteSpace(sortColumn))
                {
                    if (sortColumn.Equals("name", StringComparison.OrdinalIgnoreCase))
                        query = isAscending ? query.OrderBy(pm => pm.Name) : query.OrderByDescending(pm => pm.Name);
                    else if (sortColumn.Equals("price", StringComparison.OrdinalIgnoreCase))
                        query = isAscending ? query.OrderBy(pm => pm.Price) : query.OrderByDescending(pm => pm.Price);
                    else
                        query = isAscending ? query.OrderBy(pm => pm.CreatedAt) : query.OrderByDescending(pm => pm.CreatedAt);
                }
                else
                {
                    query = query.OrderByDescending(pm => pm.CreatedAt);
                }

                // Pagination
                int skip = (pageNumber - 1) * pageSize;
                return await query.Skip(skip).Take(pageSize).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching plan memberships: {ex.Message}", ex);
            }
        }

        public async Task<PlanMembership?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.PlanMemberships.FirstOrDefaultAsync(pm => pm.RecId == id && pm.IsActive);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching plan membership by id: {ex.Message}", ex);
            }
        }

        public async Task AddAsync(PlanMembership planMembership)
        {
            try
            {
                if (planMembership == null) throw new ArgumentNullException(nameof(planMembership));
                
                await _context.PlanMemberships.AddAsync(planMembership);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating plan membership: {ex.Message}", ex);
            }
        }

        public async Task UpdateAsync(PlanMembership planMembership)
        {
            try
            {
                if (planMembership == null) throw new ArgumentNullException(nameof(planMembership));

                _context.PlanMemberships.Update(planMembership);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating plan membership: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var planMembership = await _context.PlanMemberships.FirstOrDefaultAsync(pm => pm.RecId == id);
                if (planMembership == null) return false;

                planMembership.IsActive = false;
                _context.PlanMemberships.Update(planMembership);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting plan membership: {ex.Message}", ex);
            }
        }
    }
}
