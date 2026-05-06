using Microsoft.EntityFrameworkCore;
using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Domain.IRepositories;
using SpaceHive360.Admin.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Infrastructure.Repositories
{
    public class PaymentTermsRepository : IPaymentTermsRepository
    {

            private readonly AdminDbContext _context;

            public PaymentTermsRepository(AdminDbContext context)
            {
                _context = context;
            }
            public async Task<List<PaymentTerms>> GetAllPaymentTermsAsync(Guid userRecId)
            {
                // Step 1: Get user's company
                var userCompanyId = await _context.AdminUsers
                    .Where(u => u.RecId == userRecId)
                    .Select(u => u.FkCompany)
                    .FirstOrDefaultAsync();

                // Step 2: Get PaymentTerms for that company
                var paymentTerms = await _context.PaymentTerms
                    .Where(pt => pt.FkCompany == userCompanyId && pt.IsActive)
                    .ToListAsync();

                return paymentTerms;
            }

        public async Task<PaymentTerms?> GetByIdAsync(Guid recId, Guid userRecId)
        {
            var companyId = await _context.AdminUsers
                .Where(u => u.RecId == userRecId)
                .Select(u => u.FkCompany)
                .FirstOrDefaultAsync();

            return await _context.PaymentTerms
                .FirstOrDefaultAsync(x => x.RecId == recId && x.FkCompany == companyId && x.IsActive);
        }

        public async Task<Guid> CreateAsync(PaymentTerms entity)
        {
            _context.PaymentTerms.Add(entity);
            await _context.SaveChangesAsync();
            return entity.RecId;
        }

        public async Task UpdateAsync(PaymentTerms entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PaymentTerms entity)
        {
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;

            _context.PaymentTerms.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Guid> GetCompanyIdByUserRecIdAsync(Guid userRecId)
        {
            return await _context.AdminUsers
                .Where(u => u.RecId == userRecId)
                .Select(u => u.FkCompany)
                .FirstOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
    }
