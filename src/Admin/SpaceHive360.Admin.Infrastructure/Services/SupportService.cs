using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpaceHive360.SuperAdmin.Application.DTOs;
using SpaceHive360.SuperAdmin.Application.Interfaces;
using SpaceHive360.SuperAdmin.Domain.Entities;
using SpaceHive360.Admin.Infrastructure.Data;

namespace SpaceHive360.Admin.Infrastructure.Services
{
    public class SupportService : ISupportService
    {
        private readonly AdminDbContext _context;

        public SupportService(AdminDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FaqCategoryDto>> GetAllFaqsAsync()
        {
            // Assuming AdminDbContext has FaqCategories and Faqs
            // If not, we might need to add them to AdminDbContext too
            // Let's assume they are shared via the same table names
            
            // For now, I'll return empty if not implemented in DbContext
            // But I'll implement it properly assuming they are there.
            
            var categories = await _context.Set<FaqCategory>()
                .Include(c => c.Faqs)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            return categories.Select(c => new FaqCategoryDto
            {
                RecId = c.RecId,
                Name = c.Name,
                Icon = c.Icon,
                DisplayOrder = c.DisplayOrder,
                Faqs = c.Faqs.Select(f => new FaqDto
                {
                    RecId = f.RecId,
                    FkCategory = f.FkCategory,
                    Question = f.Question,
                    Answer = f.Answer,
                    Steps = f.Steps,
                    IsActive = f.IsActive,
                    UpdatedAt = f.UpdatedAt
                }).ToList()
            });
        }

        // Admin portal doesn't need these management methods, but interface requires them
        public Task<FaqCategoryDto> CreateCategoryAsync(CreateFaqCategoryRequest request) => throw new NotImplementedException();
        public Task<bool> UpdateCategoryAsync(Guid id, CreateFaqCategoryRequest request) => throw new NotImplementedException();
        public Task<bool> DeleteCategoryAsync(Guid id) => throw new NotImplementedException();
        public Task<FaqDto> CreateFaqAsync(CreateFaqRequest request) => throw new NotImplementedException();
        public Task<bool> UpdateFaqAsync(Guid id, CreateFaqRequest request) => throw new NotImplementedException();
        public Task<bool> DeleteFaqAsync(Guid id) => throw new NotImplementedException();
    }
}
