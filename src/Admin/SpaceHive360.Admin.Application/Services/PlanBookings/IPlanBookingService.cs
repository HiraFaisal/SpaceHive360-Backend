using Microsoft.AspNetCore.Http;
using SpaceHive360.Admin.Application.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SpaceHive360.Admin.Application.DTOs.PlanBookings;

namespace SpaceHive360.Admin.Application.Services.PlanBookings
{
    public interface IPlanBookingService
    {
        Task<ApiResponse> GetAllPlanBookingsAsync(Guid userRecId, string? search, string? filter, string sortColumn, bool isAscending, int pageNumber, int pageSize);
        Task<ApiResponse> GetPlanBookingByIdAsync(Guid id, Guid userRecId);
        Task<ApiResponse> CreatePlanBookingAsync(Guid userRecId, PlanBookingCreateDto dto, List<IFormFile>? images);
        Task<ApiResponse> UpdatePlanBookingAsync(Guid userRecId, PlanBookingUpdateDto dto, List<IFormFile>? newImages);
        Task<ApiResponse> DeletePlanBookingAsync(Guid id, Guid userRecId);
        Task<ApiResponse> GetPlanBookingStatsAsync(Guid userRecId);
    }
}
