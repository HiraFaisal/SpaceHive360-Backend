using Microsoft.EntityFrameworkCore;
using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Domain.IRepositories;
using SpaceHive360.Admin.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using SpaceHive360.Admin.Domain.Models;

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
            Guid? companyId,
            string? search,
            string? filter,
            string sortColumn,
            bool isAscending,
            int pageNumber,
            int pageSize)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                var result = await connection.QueryAsync<PlanMembership>(
                    "SELECT * FROM sp_get_plan_memberships(@CompanyId, @Search, @SortColumn, @IsAscending, @PageNumber, @PageSize)",
                    new
                    {
                        CompanyId = companyId,
                        Search = search,
                        SortColumn = sortColumn,
                        IsAscending = isAscending,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                );

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching plan memberships from SP: {ex.Message}", ex);
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
        public async Task<PlanMembershipStats> GetStatsAsync(Guid? companyId)
        {
            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    const string sql = @"
                        SELECT 
                            COUNT(*) AS TotalPlans,
                            COUNT(*) FILTER (WHERE isactive = true) AS ActivePlans,
                            COALESCE(AVG(price) FILTER (WHERE isactive = true), 0) AS AveragePrice,
                            COUNT(*) FILTER (WHERE created_at >= date_trunc('month', CURRENT_DATE)) AS NewPlansThisMonth
                        FROM tbl_plan_membership
                        WHERE (@CompanyId IS NULL OR fk_company = @CompanyId)";

                    return await connection.QuerySingleAsync<PlanMembershipStats>(sql, new { CompanyId = companyId });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching plan membership stats: {ex.Message}", ex);
            }
        }
    }
}
