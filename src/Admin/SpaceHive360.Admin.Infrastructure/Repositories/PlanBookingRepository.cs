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
    public class PlanBookingRepository : IPlanBookingRepository
    {
        private readonly AdminDbContext _context;

        public PlanBookingRepository(AdminDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<PlanBooking>> GetAllAsync(
            string? search,
            string? filter,
            string sortColumn,
            bool isAscending,
            int pageNumber,
            int pageSize)
        {
            try
            {
                var query = _context.PlanBookings.Where(pb => pb.IsActive).AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(pb => pb.Name != null && pb.Name.ToLower().Contains(search.ToLower()));
                }

                // Simple ordering
                if (!string.IsNullOrWhiteSpace(sortColumn))
                {
                    if (sortColumn.Equals("name", StringComparison.OrdinalIgnoreCase))
                        query = isAscending ? query.OrderBy(pb => pb.Name) : query.OrderByDescending(pb => pb.Name);
                    else if (sortColumn.Equals("price", StringComparison.OrdinalIgnoreCase))
                        query = isAscending ? query.OrderBy(pb => pb.Price) : query.OrderByDescending(pb => pb.Price);
                    else
                        query = isAscending ? query.OrderBy(pb => pb.CreatedAt) : query.OrderByDescending(pb => pb.CreatedAt);
                }
                else
                {
                    query = query.OrderByDescending(pb => pb.CreatedAt);
                }

                // Pagination
                int skip = (pageNumber - 1) * pageSize;
                return await query.Skip(skip).Take(pageSize).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching plan bookings: {ex.Message}", ex);
            }
        }

        public async Task<PlanBooking?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.PlanBookings.FirstOrDefaultAsync(p => p.RecId == id && p.IsActive);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching plan booking by id: {ex.Message}", ex);
            }
        }

        public async Task AddAsync(PlanBooking planBooking)
        {
            try
            {
                if (planBooking == null) throw new ArgumentNullException(nameof(planBooking));
                
                await _context.PlanBookings.AddAsync(planBooking);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating plan booking: {ex.Message}", ex);
            }
        }

        public async Task UpdateAsync(PlanBooking planBooking)
        {
            try
            {
                if (planBooking == null) throw new ArgumentNullException(nameof(planBooking));

                _context.PlanBookings.Update(planBooking);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating plan booking: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var planBooking = await _context.PlanBookings.FirstOrDefaultAsync(p => p.RecId == id);
                if (planBooking == null) return false;

                planBooking.IsActive = false;
                _context.PlanBookings.Update(planBooking);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting plan booking: {ex.Message}", ex);
            }
        }
    }
}
