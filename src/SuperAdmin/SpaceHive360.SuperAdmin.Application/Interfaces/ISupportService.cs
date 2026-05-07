using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpaceHive360.SuperAdmin.Application.DTOs;

namespace SpaceHive360.SuperAdmin.Application.Interfaces
{
    public interface ISupportService
    {
        Task<IEnumerable<FaqCategoryDto>> GetAllFaqsAsync();
        Task<FaqCategoryDto> CreateCategoryAsync(CreateFaqCategoryRequest request);
        Task<bool> UpdateCategoryAsync(Guid id, CreateFaqCategoryRequest request);
        Task<bool> DeleteCategoryAsync(Guid id);
        
        Task<FaqDto> CreateFaqAsync(CreateFaqRequest request);
        Task<bool> UpdateFaqAsync(Guid id, CreateFaqRequest request);
        Task<bool> DeleteFaqAsync(Guid id);
    }
}
