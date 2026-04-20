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
        Task<ApiResponse> GetAllPlanBookingsAsync(string? search, string? filter, string sortColumn, bool isAscending, int pageNumber, int pageSize);
        Task<ApiResponse> GetPlanBookingByIdAsync(Guid id);
        Task<ApiResponse> CreatePlanBookingAsync(PlanBookingCreateDto dto, List<IFormFile>? images);
        Task<ApiResponse> UpdatePlanBookingAsync(PlanBookingUpdateDto dto, List<IFormFile>? newImages);
        Task<ApiResponse> DeletePlanBookingAsync(Guid id);
    }
}
