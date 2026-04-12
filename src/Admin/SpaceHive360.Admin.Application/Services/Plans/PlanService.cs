using Microsoft.AspNetCore.Http;
using SpaceHive360.Admin.Application.Models;
using SpaceHive360.Admin.Application.Services.Files;
using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Domain.IRepositories;
using System.Text.Json;
using static SpaceHive360.Admin.Application.DTOs.Plans;

namespace SpaceHive360.Admin.Application.Services.Plans
{
    public class PlanService : IPlanService
    {
        private readonly IPlanRepository _planRepository;
        private readonly IFileService _fileService;

        public PlanService(IPlanRepository planRepository, IFileService fileService)
        {
            _planRepository = planRepository;
            _fileService = fileService;
        }

        // GET ALL — unchanged
        public async Task<ApiResponse> GetAllPlansAsync(
    string? search, string? filter, string sortColumn,
    bool isAscending, int pageNumber, int pageSize)
        {
            try
            {
                var result = await _planRepository.GetAllAsync(search, filter, sortColumn, isAscending, pageNumber, pageSize);

                var dtos = result.Select(MapToDto).ToList(); // ← map each plan

                return ApiResponse.SuccessResponse(dtos, "Plans fetched successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to fetch plans", 500, new List<string> { ex.Message });
            }
        }

        // GET BY ID — unchanged
        public async Task<ApiResponse> GetPlanByIdAsync(Guid id)
        {
            try
            {
                var plan = await _planRepository.GetByIdAsync(id);
                if (plan == null)
                    return ApiResponse.ErrorResponse("Plan not found", 404);

                return ApiResponse.SuccessResponse(MapToDto(plan), "Plan fetched successfully"); // ← map
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to fetch plan", 500, new List<string> { ex.Message });
            }
        }
        // CREATE — now accepts files
        public async Task<ApiResponse> CreatePlanAsync(PlanCreateDto dto, List<IFormFile>? images)
        {
            try
            {
                var imageUrls = new List<string>();

                if (images != null && images.Count > 0)
                    imageUrls = await _fileService.SaveImagesAsync(images, "plans");

                var entity = new Plan
                {
                    RecId = Guid.NewGuid(),
                    FkCompany = dto.FkCompany,
                    FkWorkspaceType = dto.FkWorkspaceType,
                    FkLocation = dto.FkLocation,
                    FkCity = dto.FkCity,
                    FkPaymentTerm = dto.FkPaymentTerm,
                    Name = dto.Name,
                    Description = dto.Description,
                    DurationMonths = dto.DurationMonths,
                    Price = dto.Price,
                    IsRecurring = dto.IsRecurring,
                    AllowCancellation = dto.AllowCancellation,
                    RequiresApproval = dto.RequiresApproval,
                    ImagesJson = JsonSerializer.Serialize(imageUrls),
                    FeaturesJson = dto.Features != null ? JsonSerializer.Serialize(dto.Features) : "[]",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _planRepository.AddAsync(entity);
                return ApiResponse.SuccessResponse(MapToDto(entity), "Plan created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to create plan", 500, new List<string> { ex.Message });
            }
        }

        // UPDATE — can replace images
        public async Task<ApiResponse> UpdatePlanAsync(PlanUpdateDto dto, List<IFormFile>? newImages)
        {
            try
            {
                var existing = await _planRepository.GetByIdAsync(dto.RecId);
                if (existing == null)
                    return ApiResponse.ErrorResponse("Plan not found", 404);

                // If new images are uploaded, delete old ones and save new ones
                if (newImages != null && newImages.Count > 0)
                {
                    var oldUrls = JsonSerializer.Deserialize<List<string>>(existing.ImagesJson ?? "[]") ?? new List<string>();
                    _fileService.DeleteImages(oldUrls);

                    var newUrls = await _fileService.SaveImagesAsync(newImages, "plans");
                    existing.ImagesJson = JsonSerializer.Serialize(newUrls);
                }

                existing.FkWorkspaceType = dto.FkWorkspaceType;
                existing.FkLocation = dto.FkLocation;
                existing.FkCity = dto.FkCity;
                existing.FkPaymentTerm = dto.FkPaymentTerm;
                existing.Name = dto.Name;
                existing.Description = dto.Description;
                existing.DurationMonths = dto.DurationMonths;
                existing.Price = dto.Price;
                existing.IsRecurring = dto.IsRecurring;
                existing.AllowCancellation = dto.AllowCancellation;
                existing.RequiresApproval = dto.RequiresApproval;
                existing.FeaturesJson = dto.Features != null ? JsonSerializer.Serialize(dto.Features) : "[]";
                existing.IsActive = dto.IsActive;
                existing.UpdatedAt = DateTime.UtcNow;

                await _planRepository.UpdateAsync(existing);
                return ApiResponse.SuccessResponse(MapToDto(existing), "Plan updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to update plan", 500, new List<string> { ex.Message });
            }
        }

        // DELETE — unchanged
        public async Task<ApiResponse> DeletePlanAsync(Guid id)
        {
            try
            {
                var isDeleted = await _planRepository.DeleteAsync(id);
                if (!isDeleted)
                    return ApiResponse.ErrorResponse("Plan not found", 404);

                return ApiResponse.SuccessResponse(null, "Plan deleted successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return ApiResponse.ErrorResponse(ex.Message, 404);
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to delete plan", 500, new List<string> { ex.Message });
            }
        }
        // Inside PlanService.cs — private helper method
        private PlanResponseDto MapToDto(Plan plan) => new PlanResponseDto
        {
            RecId = plan.RecId,
            FkCompany = plan.FkCompany,
            FkWorkspaceType = plan.FkWorkspaceType,
            FkLocation = plan.FkLocation,
            FkCity = plan.FkCity,
            FkPaymentTerm = plan.FkPaymentTerm,
            Name = plan.Name,
            Description = plan.Description,
            DurationMonths = plan.DurationMonths,
            Price = plan.Price,
            IsRecurring = plan.IsRecurring,
            AllowCancellation = plan.AllowCancellation,
            RequiresApproval = plan.RequiresApproval,
            IsActive = plan.IsActive,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt,
            Images = JsonSerializer.Deserialize<List<string>>(plan.ImagesJson ?? "[]") ?? new(),
            Features = JsonSerializer.Deserialize<List<string>>(plan.FeaturesJson ?? "[]") ?? new()
        };
    }
}