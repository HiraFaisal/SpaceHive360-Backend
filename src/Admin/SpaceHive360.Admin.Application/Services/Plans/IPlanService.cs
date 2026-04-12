using Microsoft.AspNetCore.Http;
using SpaceHive360.Admin.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static SpaceHive360.Admin.Application.DTOs.Plans;


namespace SpaceHive360.Admin.Application.Services.Plans
{
    public interface IPlanService
    {
        Task<ApiResponse> GetAllPlansAsync(string? search, string? filter, string sortColumn, bool isAscending, int pageNumber, int pageSize);
        Task<ApiResponse> GetPlanByIdAsync(Guid id);
        Task<ApiResponse> CreatePlanAsync(PlanCreateDto dto, List<IFormFile>? images);
        Task<ApiResponse> UpdatePlanAsync(PlanUpdateDto dto, List<IFormFile>? newImages);
        Task<ApiResponse> DeletePlanAsync(Guid id);
    }
}
