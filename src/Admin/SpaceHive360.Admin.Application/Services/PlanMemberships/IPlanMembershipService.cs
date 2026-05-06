using Microsoft.AspNetCore.Http;
using SpaceHive360.Admin.Application.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SpaceHive360.Admin.Application.DTOs.PlanMemberships;

namespace SpaceHive360.Admin.Application.Services.PlanMemberships
{
    public interface IPlanMembershipService
    {
        Task<ApiResponse> GetAllPlanMembershipsAsync(Guid? companyId, string? search, string? filter, string sortColumn, bool isAscending, int pageNumber, int pageSize);
        Task<ApiResponse> GetPlanMembershipByIdAsync(Guid id);
        Task<ApiResponse> CreatePlanMembershipAsync(PlanMembershipCreateDto dto, List<IFormFile>? images);
        Task<ApiResponse> UpdatePlanMembershipAsync(PlanMembershipUpdateDto dto, List<IFormFile>? newImages);
        Task<ApiResponse> DeletePlanMembershipAsync(Guid id);
        Task<ApiResponse> GetPlanMembershipStatsAsync(Guid? companyId);
    }
}
