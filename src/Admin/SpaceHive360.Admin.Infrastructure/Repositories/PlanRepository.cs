using Microsoft.EntityFrameworkCore;
using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Domain.IRepositories;
using SpaceHive360.Admin.Infrastructure.Data;
using Dapper;
using System.Data;

namespace SpaceHive360.Admin.Infrastructure.Repositories
{
    public class PlanRepository : IPlanRepository
    {
        private readonly AdminDbContext _context;

        public PlanRepository(AdminDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<List<Plan>> GetAllAsync(
    string? search,
    string? filter,
    string sortColumn,
    bool isAscending,
    int pageNumber,
    int pageSize)
        {
            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    var result = await connection.QueryAsync<Plan>(
                        "SELECT * FROM get_plans(@Search, @SortColumn, @IsAscending, @PageNumber, @PageSize)",
                        new
                        {
                            Search = search,
                            SortColumn = sortColumn,
                            IsAscending = isAscending,
                            PageNumber = pageNumber,
                            PageSize = pageSize
                        }
                    );

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching plans from function: {ex.Message}", ex);
            }
        }

        // =========================
        // GET BY ID
        // =========================
        public async Task<Plan?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.Plans
                    .FirstOrDefaultAsync(p => p.RecId == id && p.IsActive);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching plan by id: {ex.Message}", ex);
            }
        }

        // =========================
        // CREATE
        // =========================
        public async Task AddAsync(Plan plan)
        {
            try
            {
                if (plan == null)
                    throw new ArgumentNullException(nameof(plan));

                await _context.Plans.AddAsync(plan);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating plan: {ex.Message}", ex);
            }
        }

        // =========================
        // UPDATE
        // =========================
        public async Task UpdateAsync(Plan plan)
        {
            try
            {
                if (plan == null)
                    throw new ArgumentNullException(nameof(plan));

                _context.Plans.Update(plan);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating plan: {ex.Message}", ex);
            }
        }

        // =========================
        // DELETE (OPTIONAL)
        // =========================
        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var plan = await _context.Plans
                    .FirstOrDefaultAsync(p => p.RecId == id);

                if (plan == null)
                    return false;

                plan.IsActive = false;
                plan.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting plan: {ex.Message}", ex);
            }
        }
    }
}