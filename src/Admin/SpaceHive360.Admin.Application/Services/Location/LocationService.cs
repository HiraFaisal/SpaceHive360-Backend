using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Application.Services.Location
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _repository;

        public LocationService(ILocationRepository repository)
        {
            _repository = repository;
        }

        // 🔹 GET ALL (SaaS filtered)
        public async Task<List<LocationDTO>> GetAllAsync(Guid userRecId)
        {
            var data = await _repository.GetAllAsync(userRecId);

            return data.Select(x => new LocationDTO
            {
                RecId = x.RecId,
                FkCity = x.FkCity,
                Name = x.Name,
                Address = x.Address,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                IsActive = x.IsActive
            }).ToList();
        }

        // 🔹 GET BY ID (SaaS safe)
        public async Task<LocationDTO?> GetByIdAsync(Guid id, Guid userRecId)
        {
            var x = await _repository.GetByIdAsync(id, userRecId);

            if (x == null)
                return null;

            return new LocationDTO
            {
                RecId = x.RecId,
                FkCity = x.FkCity,
                Name = x.Name,
                Address = x.Address,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                IsActive = x.IsActive
            };
        }

        // 🔹 CREATE (SaaS safe)
        public async Task<Guid> CreateAsync(LocationCreateDTO dto, Guid userRecId)
        {
            var companyId = await _repository.GetCompanyIdByUserRecIdAsync(userRecId);

            var entity = new Admin.Domain.Entities.Location
            {
                RecId = Guid.NewGuid(),
                FkCompany = companyId,
                FkCity = dto.FkCity,
                Name = dto.Name,
                Address = dto.Address,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            return await _repository.CreateAsync(entity);
        }

        // 🔹 UPDATE
        public async Task<bool> UpdateAsync(LocationUpdateDTO dto, Guid userRecId)
        {
            var entity = await _repository.GetByIdAsync(dto.RecId, userRecId);

            if (entity == null)
                return false;

            entity.FkCity = dto.FkCity;
            entity.Name = dto.Name;
            entity.Address = dto.Address;
            entity.Latitude = dto.Latitude;
            entity.Longitude = dto.Longitude;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);

            return true;
        }

        // 🔹 DELETE (soft delete)
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
