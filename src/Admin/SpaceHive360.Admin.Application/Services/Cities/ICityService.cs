using SpaceHive360.Admin.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Application.Services.Cities
{
    public interface ICityService
    {
        Task<List<CityDTO>> GetAllAsync(Guid userRecId);
        Task<CityDTO?> GetByIdAsync(Guid id, Guid userRecId);
        Task<Guid> CreateAsync(CityCreateDTO dto, Guid userRecId);
        Task<bool> UpdateAsync(CityUpdateDTO dto, Guid userRecId);
        Task<bool> DeleteAsync(Guid id, Guid userRecId);
    }

    public class CityDTO
    {
        public Guid RecId { get; set; }
        public string Name { get; set; } = null!;
        public string? Country { get; set; }
        public bool IsActive { get; set; }
    }

    public class CityCreateDTO
    {
        public string Name { get; set; } = null!;
        public string? Country { get; set; }
    }

    public class CityUpdateDTO
    {
        public Guid RecId { get; set; }
        public string Name { get; set; } = null!;
        public string? Country { get; set; }
        public bool IsActive { get; set; }
    }
}
