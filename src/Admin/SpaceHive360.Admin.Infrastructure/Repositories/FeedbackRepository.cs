using Dapper;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Application.IRepositories;
using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SpaceHive360.Admin.Infrastructure.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly AdminDbContext _context;

        public FeedbackRepository(AdminDbContext context)
        {
            _context = context;
        }

        // 🔹 Get CompanyId from logged-in user
        public async Task<Guid> GetCompanyIdByUserRecIdAsync(Guid userRecId)
        {
            return await _context.AdminUsers
                .Where(u => u.RecId == userRecId)
                .Select(u => u.FkCompany)
                .FirstOrDefaultAsync();
        }

        //// 🔹 GET ALL (SaaS FILTERED)
        //public async Task<List<Feedback>> GetAllAsync(Guid userRecId)
        //{
        //    var companyId = await GetCompanyIdByUserRecIdAsync(userRecId);

        //    return await _context.Feedbacks
        //        .Where(x => x.FkCompany == companyId && x.IsActive)
        //        .OrderByDescending(x => x.CreatedAt)
        //        .ToListAsync();
        //}

        //// 🔹 GET BY ID (SaaS SAFE)
        //public async Task<Feedback?> GetByIdAsync(Guid id, Guid userRecId)
        //{
        //    var companyId = await GetCompanyIdByUserRecIdAsync(userRecId);

        //    return await _context.Feedbacks
        //        .FirstOrDefaultAsync(x =>
        //            x.RecId == id &&
        //            x.FkCompany == companyId &&
        //            x.IsActive);
        //}

        public async Task<List<FeedbackDTO>> GetFeedbacksAsync(Guid? recId, Guid userRecId)
        {
            try
            {
                var companyId = await GetCompanyIdByUserRecIdAsync(userRecId);

                using (var connection = _context.Database.GetDbConnection())
                {
                    var result = await connection.QueryAsync(
                        "SELECT * FROM public.fn_get_feedbacks(@p_company_id, @p_rec_id)",
                        new
                        {
                            p_company_id = companyId,
                            p_rec_id = recId
                        }
                    );

                    var feedbackList = new List<FeedbackDTO>();

                    foreach (var row in result)
                    {
                        feedbackList.Add(new FeedbackDTO
                        {
                            RecId = row.rec_id,
                            FkLocation = row.fk_location,
                            LocationName = row.location_name,
                            FkPayment = row.fk_payment,
                            MemberName = row.member_name,
                            MemberEmail = row.member_email,
                            Category = row.category,
                            Experience = row.experience,
                            Comments = row.comments,
                            Rating = row.rating,
                            IsActive = row.is_active,
                            CreatedAt = row.created_at
                        });
                    }

                    return feedbackList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching feedbacks: {ex.Message}", ex);
            }
        }
    }
}