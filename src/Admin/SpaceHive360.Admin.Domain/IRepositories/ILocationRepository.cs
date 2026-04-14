using SpaceHive360.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Domain.IRepositories
{
    public interface ILocationRepository
    {
        Task<List<Location>> GetAllAsync(Guid userRecId);

        Task<Location?> GetByIdAsync(Guid id, Guid userRecId);

        Task<Guid> CreateAsync(Location entity);

        Task UpdateAsync(Location entity);

        Task DeleteAsync(Location entity);

        Task<Guid> GetCompanyIdByUserRecIdAsync(Guid userRecId);
    }
}
