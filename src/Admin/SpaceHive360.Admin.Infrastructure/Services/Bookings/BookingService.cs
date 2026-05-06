using Microsoft.EntityFrameworkCore;
using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Application.Models;
using SpaceHive360.Admin.Application.Services.Bookings;
using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Infrastructure.Services.Bookings
{
    public class BookingService : IBookingService
    {
        private readonly AdminDbContext _context;

        public BookingService(AdminDbContext context)
        {
            _context = context;
        }

        private async Task<Guid?> GetCompanyIdAsync(Guid adminId)
        {
            var admin = await _context.AdminUsers.FindAsync(adminId);
            return admin?.FkCompany;
        }

        public async Task<ApiResponse> GetBookingsAsync(Guid adminId, string? search, int pageNumber, int pageSize)
        {
            var companyId = await GetCompanyIdAsync(adminId);
            if (companyId == null) return ApiResponse.ErrorResponse("Company not found");

            var query = (from b in _context.MemberBookings
                        join u in _context.MemberUsers on b.FkMemberUser equals u.RecId
                        join pb in _context.PlanBookings on b.FkPlan equals pb.RecId into pbs from pb in pbs.DefaultIfEmpty()
                        join pm in _context.PlanMemberships on b.FkPlan equals pm.RecId into pms from pm in pms.DefaultIfEmpty()
                        where (pb != null && pb.FkCompany == companyId) || (pm != null && pm.FkCompany == companyId)
                        select new
                        {
                            b.RecId,
                            b.BookingDate,
                            b.BookingStatus,
                            b.TotalAmount,
                            b.PaymentStatus,
                            MemberName = u.FullName,
                            MemberEmail = u.Email,
                            PlanName = pb != null ? pb.Name : (pm != null ? pm.Name : "N/A"),
                            PlanType = pb != null ? "Booking" : (pm != null ? "Membership" : "N/A")
                        });

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.MemberName.Contains(search) || x.PlanName.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return ApiResponse.SuccessResponse(new { totalCount, items });
        }

        public async Task<ApiResponse> GetPaymentHistoryAsync(Guid adminId, int pageNumber, int pageSize)
        {
            var companyId = await GetCompanyIdAsync(adminId);
            if (companyId == null) return ApiResponse.ErrorResponse("Company not found");

            var query = (from b in _context.MemberBookings
                        join u in _context.MemberUsers on b.FkMemberUser equals u.RecId
                        join pb in _context.PlanBookings on b.FkPlan equals pb.RecId into pbs from pb in pbs.DefaultIfEmpty()
                        join pm in _context.PlanMemberships on b.FkPlan equals pm.RecId into pms from pm in pms.DefaultIfEmpty()
                        where ((pb != null && pb.FkCompany == companyId) || (pm != null && pm.FkCompany == companyId))
                        && b.PaymentStatus == "Completed"
                        select new
                        {
                            b.RecId,
                            b.BookingDate,
                            b.TotalAmount,
                            b.PaymentId,
                            MemberName = u.FullName,
                            PlanName = pb != null ? pb.Name : (pm != null ? pm.Name : "N/A")
                        });

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return ApiResponse.SuccessResponse(new { totalCount, items });
        }

        public async Task<ApiResponse> GetCalendarBookingsAsync(Guid adminId)
        {
            var companyId = await GetCompanyIdAsync(adminId);
            if (companyId == null) return ApiResponse.ErrorResponse("Company not found");

            var items = await (from b in _context.MemberBookings
                              join pb in _context.PlanBookings on b.FkPlan equals pb.RecId into pbs from pb in pbs.DefaultIfEmpty()
                              join pm in _context.PlanMemberships on b.FkPlan equals pm.RecId into pms from pm in pms.DefaultIfEmpty()
                              where (pb != null && pb.FkCompany == companyId) || (pm != null && pm.FkCompany == companyId)
                              select new
                              {
                                  b.RecId,
                                  Date = b.BookingDate,
                                  Title = pb != null ? pb.Name : (pm != null ? pm.Name : "N/A"),
                                  PlanType = pb != null ? "Booking" : (pm != null ? "Membership" : "N/A"),
                                  Status = b.BookingStatus
                              }).ToListAsync();

            return ApiResponse.SuccessResponse(items);
        }

        public async Task<ApiResponse> GetDashboardStatsAsync(Guid adminId)
        {
            var companyId = await GetCompanyIdAsync(adminId);
            if (companyId == null) return ApiResponse.ErrorResponse("Company not found");

            var totalRevenue = await (from b in _context.MemberBookings
                                      join pb in _context.PlanBookings on b.FkPlan equals pb.RecId into pbs from pb in pbs.DefaultIfEmpty()
                                      join pm in _context.PlanMemberships on b.FkPlan equals pm.RecId into pms from pm in pms.DefaultIfEmpty()
                                      where ((pb != null && pb.FkCompany == companyId) || (pm != null && pm.FkCompany == companyId))
                                      && b.PaymentStatus == "Completed"
                                      select b.TotalAmount).SumAsync() ?? 0;

            var activeMembers = await (from b in _context.MemberBookings
                                       join pb in _context.PlanBookings on b.FkPlan equals pb.RecId into pbs from pb in pbs.DefaultIfEmpty()
                                       join pm in _context.PlanMemberships on b.FkPlan equals pm.RecId into pms from pm in pms.DefaultIfEmpty()
                                       where ((pb != null && pb.FkCompany == companyId) || (pm != null && pm.FkCompany == companyId))
                                       select b.FkMemberUser).Distinct().CountAsync();

            var totalBookingsToday = await (from b in _context.MemberBookings
                                            join pb in _context.PlanBookings on b.FkPlan equals pb.RecId into pbs from pb in pbs.DefaultIfEmpty()
                                            join pm in _context.PlanMemberships on b.FkPlan equals pm.RecId into pms from pm in pms.DefaultIfEmpty()
                                            where ((pb != null && pb.FkCompany == companyId) || (pm != null && pm.FkCompany == companyId))
                                            && b.BookingDate.HasValue && b.BookingDate.Value.Date == DateTime.UtcNow.Date
                                            select b).CountAsync();

            return ApiResponse.SuccessResponse(new
            {
                totalRevenue,
                activeMembers,
                totalBookingsToday
            });
        }
    }
}
