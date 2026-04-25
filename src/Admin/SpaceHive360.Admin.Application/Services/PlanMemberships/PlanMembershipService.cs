using Microsoft.AspNetCore.Http;
using SpaceHive360.Admin.Application.Models;
using SpaceHive360.Admin.Application.Services.Files;
using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static SpaceHive360.Admin.Application.DTOs.PlanMemberships;

namespace SpaceHive360.Admin.Application.Services.PlanMemberships
{
    public class PlanMembershipService : IPlanMembershipService
    {
        private readonly IPlanMembershipRepository _planMembershipRepository;
        private readonly IFileService _fileService;

        public PlanMembershipService(IPlanMembershipRepository planMembershipRepository, IFileService fileService)
        {
            _planMembershipRepository = planMembershipRepository;
            _fileService = fileService;
        }

        public async Task<ApiResponse> GetAllPlanMembershipsAsync(Guid? companyId, string? search, string? filter, string sortColumn, bool isAscending, int pageNumber, int pageSize)
        {
            try
            {
                var result = await _planMembershipRepository.GetAllAsync(companyId, search, filter, sortColumn, isAscending, pageNumber, pageSize);
                var dtos = result.Select(MapToDto).ToList();
                return ApiResponse.SuccessResponse(dtos, "Plan memberships fetched successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to fetch plan memberships", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse> GetPlanMembershipByIdAsync(Guid id)
        {
            try
            {
                var planMembership = await _planMembershipRepository.GetByIdAsync(id);
                if (planMembership == null) return ApiResponse.ErrorResponse("Plan membership not found", 404);

                return ApiResponse.SuccessResponse(MapToDto(planMembership), "Plan membership fetched successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to fetch plan membership", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse> CreatePlanMembershipAsync(PlanMembershipCreateDto dto, List<IFormFile>? images)
        {
            try
            {
                var imageUrls = new List<string>();

                if (images != null && images.Count > 0)
                    imageUrls = await _fileService.SaveImagesAsync(images, "planmemberships");

                var entity = new PlanMembership
                {
                    RecId = Guid.NewGuid(),
                    FkCompany = dto.FkCompany,
                    FkWorkspaceType = dto.FkWorkspaceType,
                    FkWorkspace = dto.FkWorkspace,
                    FkLocation = dto.FkLocation,
                    Name = dto.Name,
                    Description = dto.Description,
                    DurationType = dto.DurationType,
                    DurationValue = dto.DurationValue,
                    FkPaymentTerm = dto.FkPaymentTerm,
                    Price = dto.Price,
                    IsRecurring = dto.IsRecurring,
                    AllowCancellation = dto.AllowCancellation,
                    RequiresApproval = dto.RequiresApproval,
                    Images = JsonSerializer.Serialize(imageUrls),
                    Features = dto.Features != null ? JsonSerializer.Serialize(dto.Features) : "[]",
                    PlanCategory = dto.PlanCategory ?? "membership",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _planMembershipRepository.AddAsync(entity);
                return ApiResponse.SuccessResponse(MapToDto(entity), "Plan membership created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to create plan membership", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse> UpdatePlanMembershipAsync(PlanMembershipUpdateDto dto, List<IFormFile>? newImages)
        {
            try
            {
                var existing = await _planMembershipRepository.GetByIdAsync(dto.RecId);
                if (existing == null) return ApiResponse.ErrorResponse("Plan membership not found", 404);

                if (newImages != null && newImages.Count > 0)
                {
                    var oldUrls = JsonSerializer.Deserialize<List<string>>(existing.Images ?? "[]") ?? new List<string>();
                    _fileService.DeleteImages(oldUrls);

                    var newUrls = await _fileService.SaveImagesAsync(newImages, "planmemberships");
                    existing.Images = JsonSerializer.Serialize(newUrls);
                }

                existing.FkCompany = dto.FkCompany ?? existing.FkCompany;
                existing.FkWorkspaceType = dto.FkWorkspaceType;
                existing.FkWorkspace = dto.FkWorkspace;
                existing.FkLocation = dto.FkLocation;
                existing.Name = dto.Name;
                existing.Description = dto.Description;
                existing.DurationType = dto.DurationType;
                existing.DurationValue = dto.DurationValue;
                existing.FkPaymentTerm = dto.FkPaymentTerm;
                existing.Price = dto.Price;
                existing.IsRecurring = dto.IsRecurring;
                existing.AllowCancellation = dto.AllowCancellation;
                existing.RequiresApproval = dto.RequiresApproval;
                existing.Features = dto.Features != null ? JsonSerializer.Serialize(dto.Features) : existing.Features;
                existing.UpdatedAt = DateTime.UtcNow;

                await _planMembershipRepository.UpdateAsync(existing);
                return ApiResponse.SuccessResponse(MapToDto(existing), "Plan membership updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to update plan membership", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse> DeletePlanMembershipAsync(Guid id)
        {
            try
            {
                var isDeleted = await _planMembershipRepository.DeleteAsync(id);
                if (!isDeleted) return ApiResponse.ErrorResponse("Plan membership not found", 404);

                return ApiResponse.SuccessResponse(null, "Plan membership deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to delete plan membership", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse> GetPlanMembershipStatsAsync(Guid? companyId)
        {
            try
            {
                var stats = await _planMembershipRepository.GetStatsAsync(companyId);
                var dto = new PlanMembershipStatsDto
                {
                    TotalPlans = stats.TotalPlans,
                    ActivePlans = stats.ActivePlans,
                    AveragePrice = stats.AveragePrice,
                    NewPlansThisMonth = stats.NewPlansThisMonth
                };
                return ApiResponse.SuccessResponse(dto, "Plan membership stats fetched successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to fetch plan membership stats", 500, new List<string> { ex.Message });
            }
        }

        private PlanMembershipDto MapToDto(PlanMembership planMembership) => new PlanMembershipDto
        {
            RecId = planMembership.RecId,
            FkCompany = planMembership.FkCompany,
            FkWorkspaceType = planMembership.FkWorkspaceType,
            FkWorkspace = planMembership.FkWorkspace,
            FkLocation = planMembership.FkLocation,
            Name = planMembership.Name!,
            Description = planMembership.Description,
            DurationType = planMembership.DurationType,
            DurationValue = planMembership.DurationValue,
            FkPaymentTerm = planMembership.FkPaymentTerm,
            Price = planMembership.Price,
            IsRecurring = planMembership.IsRecurring,
            AllowCancellation = planMembership.AllowCancellation,
            RequiresApproval = planMembership.RequiresApproval,
            PlanCategory = planMembership.PlanCategory,
            IsActive = planMembership.IsActive,
            CreatedAt = planMembership.CreatedAt,
            UpdatedAt = planMembership.UpdatedAt,
            Images = JsonSerializer.Deserialize<List<string>>(planMembership.Images ?? "[]") ?? new(),
            Features = JsonSerializer.Deserialize<List<string>>(planMembership.Features ?? "[]") ?? new()
        };
    }
}
