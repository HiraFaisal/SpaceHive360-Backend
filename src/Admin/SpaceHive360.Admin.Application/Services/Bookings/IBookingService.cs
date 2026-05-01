using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Application.Models;
using System;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Application.Services.Bookings
{
    public interface IBookingService
    {
        Task<ApiResponse> GetBookingsAsync(Guid adminId, string? search, int pageNumber, int pageSize);
        Task<ApiResponse> GetPaymentHistoryAsync(Guid adminId, int pageNumber, int pageSize);
        Task<ApiResponse> GetCalendarBookingsAsync(Guid adminId);
        Task<ApiResponse> GetDashboardStatsAsync(Guid adminId);
    }
}
