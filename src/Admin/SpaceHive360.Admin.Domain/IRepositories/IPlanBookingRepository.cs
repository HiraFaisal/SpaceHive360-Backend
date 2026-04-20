using SpaceHive360.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Domain.IRepositories
{
    public interface IPlanBookingRepository
    {
        Task<List<PlanBooking>> GetAllAsync(string? search, string? filter, string sortColumn, bool isAscending, int pageNumber, int pageSize);
        Task<PlanBooking?> GetByIdAsync(Guid id);
        Task AddAsync(PlanBooking planBooking);
        Task UpdateAsync(PlanBooking planBooking);
        Task<bool> DeleteAsync(Guid id);
    }
}
