using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpaceHive360.SuperAdmin.Application.DTOs;
using SpaceHive360.SuperAdmin.Application.Interfaces;
using SpaceHive360.SuperAdmin.Domain.Entities;
using SpaceHive360.SuperAdmin.Infrastructure.Data;

namespace SpaceHive360.SuperAdmin.Infrastructure.Services
{
    public class SupportService : ISupportService
    {
        private readonly SuperAdminDbContext _context;

        public SupportService(SuperAdminDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FaqCategoryDto>> GetAllFaqsAsync()
        {
            var categories = await _context.FaqCategories
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

        public async Task<FaqCategoryDto> CreateCategoryAsync(CreateFaqCategoryRequest request)
        {
            var category = new FaqCategory
            {
                Name = request.Name,
                Icon = request.Icon,
                DisplayOrder = request.DisplayOrder
            };

            _context.FaqCategories.Add(category);
            await _context.SaveChangesAsync();

            return new FaqCategoryDto
            {
                RecId = category.RecId,
                Name = category.Name,
                Icon = category.Icon,
                DisplayOrder = category.DisplayOrder
            };
        }

        public async Task<bool> UpdateCategoryAsync(Guid id, CreateFaqCategoryRequest request)
        {
            var category = await _context.FaqCategories.FindAsync(id);
            if (category == null) return false;

            category.Name = request.Name;
            category.Icon = request.Icon;
            category.DisplayOrder = request.DisplayOrder;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _context.FaqCategories.FindAsync(id);
            if (category == null) return false;

            // Manually delete associated FAQs since we removed DB-level cascade
            var associatedFaqs = await _context.Faqs.Where(f => f.FkCategory == id).ToListAsync();
            if (associatedFaqs.Any())
            {
                _context.Faqs.RemoveRange(associatedFaqs);
            }

            _context.FaqCategories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<FaqDto> CreateFaqAsync(CreateFaqRequest request)
        {
            var faq = new Faq
            {
                FkCategory = request.FkCategory,
                Question = request.Question,
                Answer = request.Answer,
                Steps = request.Steps
            };

            _context.Faqs.Add(faq);
            await _context.SaveChangesAsync();

            return new FaqDto
            {
                RecId = faq.RecId,
                FkCategory = faq.FkCategory,
                Question = faq.Question,
                Answer = faq.Answer,
                Steps = faq.Steps,
                IsActive = faq.IsActive,
                UpdatedAt = faq.UpdatedAt
            };
        }

        public async Task<bool> UpdateFaqAsync(Guid id, CreateFaqRequest request)
        {
            var faq = await _context.Faqs.FindAsync(id);
            if (faq == null) return false;

            faq.FkCategory = request.FkCategory;
            faq.Question = request.Question;
            faq.Answer = request.Answer;
            faq.Steps = request.Steps;
            faq.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFaqAsync(Guid id)
        {
            var faq = await _context.Faqs.FindAsync(id);
            if (faq == null) return false;

            _context.Faqs.Remove(faq);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
