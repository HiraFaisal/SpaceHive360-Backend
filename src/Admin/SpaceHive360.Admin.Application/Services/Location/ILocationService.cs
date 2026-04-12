using SpaceHive360.Admin.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Application.Services.Location
{
    public interface ILocationService
    {
        Task<List<LocationDTO>> GetAllAsync(Guid userRecId);

        Task<LocationDTO?> GetByIdAsync(Guid id, Guid userRecId);

        Task<Guid> CreateAsync(LocationCreateDTO dto, Guid userRecId);

        Task<bool> UpdateAsync(LocationUpdateDTO dto, Guid userRecId);

        Task<bool> DeleteAsync(Guid id, Guid userRecId);
    }
}
