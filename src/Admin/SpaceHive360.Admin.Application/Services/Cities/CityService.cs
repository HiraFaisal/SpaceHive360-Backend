using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Application.Services.Cities
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _repository;

        public CityService(ICityRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CityDTO>> GetAllAsync(Guid userRecId)
        {
            var data = await _repository.GetAllAsync(userRecId);

            return data.Select(x => new CityDTO
            {
                RecId = x.RecId,
                Name = x.Name,
                Country = x.Country,
                IsActive = x.IsActive
            }).ToList();
        }

        public async Task<CityDTO?> GetByIdAsync(Guid id, Guid userRecId)
        {
            var x = await _repository.GetByIdAsync(id, userRecId);

            if (x == null)
                return null;

            return new CityDTO
            {
                RecId = x.RecId,
                Name = x.Name,
                Country = x.Country,
                IsActive = x.IsActive
            };
        }

        public async Task<Guid> CreateAsync(CityCreateDTO dto, Guid userRecId)
        {
            var entity = new City
            {
                RecId = Guid.NewGuid(),
                Name = dto.Name,
                Country = dto.Country,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return await _repository.CreateAsync(entity);
        }

        public async Task<bool> UpdateAsync(CityUpdateDTO dto, Guid userRecId)
        {
            var entity = await _repository.GetByIdAsync(dto.RecId, userRecId);

            if (entity == null)
                return false;

            entity.Name = dto.Name;
            entity.Country = dto.Country;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userRecId)
        {
            var entity = await _repository.GetByIdAsync(id, userRecId);

            if (entity == null)
                return false;

            await _repository.DeleteAsync(entity);

            return true;
        }
    }
}
