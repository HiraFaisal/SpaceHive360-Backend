using SpaceHive360.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Domain.IRepositories
{
    public interface ICityRepository
    {
        Task<List<City>> GetAllAsync(Guid userRecId);
        Task<City?> GetByIdAsync(Guid id, Guid userRecId);
        Task<Guid> CreateAsync(City entity);
        Task UpdateAsync(City entity);
        Task DeleteAsync(City entity);
        Task<Guid> GetCompanyIdByUserRecIdAsync(Guid userRecId);
    }
}
