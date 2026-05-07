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

            // Fetch payments from Bookings
            var bookingPayments = await (from b in _context.MemberBookings
                                       join u in _context.MemberUsers on b.FkMemberUser equals u.RecId
                                       join pb in _context.PlanBookings on b.FkPlan equals pb.RecId into pbs from pb in pbs.DefaultIfEmpty()
                                       join pm in _context.PlanMemberships on b.FkPlan equals pm.RecId into pms from pm in pms.DefaultIfEmpty()
                                       where ((pb != null && pb.FkCompany == companyId) || (pm != null && pm.FkCompany == companyId))
                                       && b.PaymentStatus == "Completed"
                                       select new
                                       {
                                           b.RecId,
                                           BookingDate = b.BookingDate ?? DateTime.UtcNow,
                                           TotalAmount = b.TotalAmount ?? 0m,
                                           PaymentId = b.PaymentId,
                                           MemberName = u.FullName,
                                           PlanName = pb != null ? pb.Name : (pm != null ? pm.Name : "N/A"),
                                           Type = "Booking"
                                       }).ToListAsync();

            // Fetch payments from Memberships
            var membershipPayments = await (from m in _context.MemberMemberships
                                          join u in _context.MemberUsers on m.FkMemberUser equals u.RecId
                                          join p in _context.PlanMemberships on m.FkPlan equals p.RecId
                                          join pay in _context.MemberPayments on m.FkPayment equals pay.RecId
                                          where p.FkCompany == companyId && pay.PaymentStatus == "Completed"
                                          select new
                                          {
                                              m.RecId,
                                              BookingDate = m.CreatedAt,
                                              TotalAmount = (decimal?)m.Amount ?? 0m,
                                              PaymentId = pay.TransactionId,
                                              MemberName = u.FullName,
                                              PlanName = p.Name ?? "Membership Plan",
                                              Type = "Membership"
                                          }).ToListAsync();

            // Combine and sort
            var allPayments = bookingPayments.Concat(membershipPayments)
                .OrderByDescending(p => p.BookingDate)
                .ToList();

            var totalCount = allPayments.Count;
            var items = allPayments.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

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

            var bookingRevenue = await (from b in _context.MemberBookings
                                       join pb in _context.PlanBookings on b.FkPlan equals pb.RecId into pbs from pb in pbs.DefaultIfEmpty()
                                       join pm in _context.PlanMemberships on b.FkPlan equals pm.RecId into pms from pm in pms.DefaultIfEmpty()
                                       where ((pb != null && pb.FkCompany == companyId) || (pm != null && pm.FkCompany == companyId))
                                       && b.PaymentStatus == "Completed"
                                       select b.TotalAmount).SumAsync() ?? 0m;

            var membershipRevenue = await (from m in _context.MemberMemberships
                                         join p in _context.PlanMemberships on m.FkPlan equals p.RecId
                                         join pay in _context.MemberPayments on m.FkPayment equals pay.RecId
                                         where p.FkCompany == companyId && pay.PaymentStatus == "Completed"
                                         select m.Amount).SumAsync();

            var totalRevenue = bookingRevenue + membershipRevenue;

            var bookingUsers = await (from b in _context.MemberBookings
                                     join pb in _context.PlanBookings on b.FkPlan equals pb.RecId into pbs from pb in pbs.DefaultIfEmpty()
                                     join pm in _context.PlanMemberships on b.FkPlan equals pm.RecId into pms from pm in pms.DefaultIfEmpty()
                                     where ((pb != null && pb.FkCompany == companyId) || (pm != null && pm.FkCompany == companyId))
                                     select b.FkMemberUser).Distinct().ToListAsync();

            var membershipUsers = await (from m in _context.MemberMemberships
                                       join p in _context.PlanMemberships on m.FkPlan equals p.RecId
                                       where p.FkCompany == companyId
                                       select m.FkMemberUser).Distinct().ToListAsync();

            var activeMembers = bookingUsers.Union(membershipUsers).Count();

            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var totalBookingsToday = await (from b in _context.MemberBookings
                                            join pb in _context.PlanBookings on b.FkPlan equals pb.RecId into pbs from pb in pbs.DefaultIfEmpty()
                                            join pm in _context.PlanMemberships on b.FkPlan equals pm.RecId into pms from pm in pms.DefaultIfEmpty()
                                            where ((pb != null && pb.FkCompany == companyId) || (pm != null && pm.FkCompany == companyId))
                                            && b.BookingDate >= today && b.BookingDate < tomorrow
                                            select b).CountAsync();

            // 🟢 New Occupancy Logic
            var workspaces = await _context.Workspaces.Where(w => w.FkCompany == companyId && w.IsActive).ToListAsync();
            int totalCapacity = 0;
            int currentOccupancy = 0;

            var now = DateTime.UtcNow;

            foreach (var ws in workspaces)
            {
                if (ws.InventoryType == "UNIT")
                {
                    totalCapacity += 1;
                    var isOccupied = await (from b in _context.MemberBookings
                                           join d in _context.BookingDetails on b.RecId equals d.FkBooking
                                           join pb in _context.PlanBookings on b.FkPlan equals pb.RecId
                                           where pb.FkWorkspace == ws.RecId && b.BookingStatus == "Confirmed"
                                           && d.StartTime <= now && d.EndTime >= now
                                           select b).AnyAsync() 
                                   || await _context.MemberMemberships
                                           .AnyAsync(m => m.FkPlan == ws.RecId && m.MembershipStatus == "Active" 
                                                     && m.StartDate <= now && m.EndDate >= now);
                    
                    if (isOccupied) currentOccupancy += 1;
                }
                else // SEAT
                {
                    totalCapacity += ws.Capacity ?? 0;
                    
                    var bookedSeats = await (from b in _context.MemberBookings
                                           join d in _context.BookingDetails on b.RecId equals d.FkBooking
                                           join pb in _context.PlanBookings on b.FkPlan equals pb.RecId
                                           where pb.FkWorkspace == ws.RecId && b.BookingStatus == "Confirmed"
                                           && d.StartTime <= now && d.EndTime >= now
                                           select b).CountAsync();

                    var membershipSeats = await _context.MemberMemberships
                                           .CountAsync(m => m.FkPlan == ws.RecId && m.MembershipStatus == "Active"
                                                     && m.StartDate <= now && m.EndDate >= now);

                    currentOccupancy += (bookedSeats + membershipSeats);
                }
            }

            return ApiResponse.SuccessResponse(new
            {
                totalRevenue,
                activeMembers,
                totalBookingsToday,
                occupancy = new {
                    used = currentOccupancy,
                    total = totalCapacity,
                    percentage = totalCapacity > 0 ? Math.Round((double)currentOccupancy / totalCapacity * 100, 2) : 0
                }
            });
        }
    }
}
