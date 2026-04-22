using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Domain.IRepositories;
using SpaceHive360.Admin.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Application.Services.PaymentTerms
{
    public class PaymentTermsService : IPaymentTermsService
    {
        private readonly IPaymentTermsRepository _repository;



        public PaymentTermsService(IPaymentTermsRepository repository)
        {
            _repository = repository;
            
        }
        public async Task<List<PaymentTermsDTO>> GetAllPaymentTermsAsync(Guid userRecId)
        {
            var data = await _repository.GetAllPaymentTermsAsync(userRecId);

            // Mapping Entity → DTO
            var result = data.Select(x => new PaymentTermsDTO
            {
                RecId = x.RecId,
                Name = x.Name,
                Description = x.Description,
                BillingCycleMonths = x.BillingCycleMonths,
                IsActive = x.IsActive
            }).ToList();

            return result;
        }

        public async Task<PaymentTermsDTO?> GetByIdAsync(Guid recId, Guid userRecId)
        {
            var entity = await _repository.GetByIdAsync(recId, userRecId);

            if (entity == null) return null;

            return new PaymentTermsDTO
            {
                RecId = entity.RecId,
                Name = entity.Name,
                Description = entity.Description,
                BillingCycleMonths = entity.BillingCycleMonths,
                IsActive = entity.IsActive
            };
        }

        public async Task<Guid> CreateAsync(PaymentTermsCreateDTO dto, Guid userRecId)
        {
            var companyId = await _repository.GetCompanyIdByUserRecIdAsync(userRecId);

            var entity = new Domain.Entities.PaymentTerms
            {
                RecId = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                BillingCycleMonths = dto.BillingCycleMonths,
                IsActive = dto.IsActive,
                FkCompany = companyId,
                CreatedAt = DateTime.UtcNow
            };

            return await _repository.CreateAsync(entity);
        }

        public async Task<bool> UpdateAsync(PaymentTermsUpdateDTO dto, Guid userRecId)
        {
            var entity = await _repository.GetByIdAsync(dto.RecId, userRecId);

            if (entity == null) return false;

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.BillingCycleMonths = dto.BillingCycleMonths;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid recId, Guid userRecId)
        {
            var entity = await _repository.GetByIdAsync(recId, userRecId);

            if (entity == null) return false;

            await _repository.DeleteAsync(entity);
            return true;
        }
    }
}
