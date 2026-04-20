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
using static SpaceHive360.Admin.Application.DTOs.PlanBookings;

namespace SpaceHive360.Admin.Application.Services.PlanBookings
{
    public class PlanBookingService : IPlanBookingService
    {
        private readonly IPlanBookingRepository _planBookingRepository;
        private readonly IFileService _fileService;

        public PlanBookingService(IPlanBookingRepository planBookingRepository, IFileService fileService)
        {
            _planBookingRepository = planBookingRepository;
            _fileService = fileService;
        }

        public async Task<ApiResponse> GetAllPlanBookingsAsync(string? search, string? filter, string sortColumn, bool isAscending, int pageNumber, int pageSize)
        {
            try
            {
                var result = await _planBookingRepository.GetAllAsync(search, filter, sortColumn, isAscending, pageNumber, pageSize);
                var dtos = result.Select(MapToDto).ToList();
                return ApiResponse.SuccessResponse(dtos, "Plan bookings fetched successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to fetch plan bookings", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse> GetPlanBookingByIdAsync(Guid id)
        {
            try
            {
                var planBooking = await _planBookingRepository.GetByIdAsync(id);
                if (planBooking == null) return ApiResponse.ErrorResponse("Plan booking not found", 404);

                return ApiResponse.SuccessResponse(MapToDto(planBooking), "Plan booking fetched successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to fetch plan booking", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse> CreatePlanBookingAsync(PlanBookingCreateDto dto, List<IFormFile>? images)
        {
            try
            {
                var imageUrls = new List<string>();

                if (images != null && images.Count > 0)
                    imageUrls = await _fileService.SaveImagesAsync(images, "planbookings");

                var entity = new PlanBooking
                {
                    RecId = Guid.NewGuid(),
                    FkCompany = dto.FkCompany,
                    FkWorkspace = dto.FkWorkspace,
                    FkWorkspaceType = dto.FkWorkspaceType,
                    FkLocation = dto.FkLocation,
                    Name = dto.Name,
                    Description = dto.Description,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    MinDurationMinutes = dto.MinDurationMinutes,
                    MaxDurationMinutes = dto.MaxDurationMinutes,
                    PriceType = dto.PriceType,
                    Price = dto.Price,
                    AllowCancellation = dto.AllowCancellation,
                    RequiresApproval = dto.RequiresApproval,
                    Images = JsonSerializer.Serialize(imageUrls),
                    Features = dto.Features != null ? JsonSerializer.Serialize(dto.Features) : "[]",
                    AvailableDays = dto.AvailableDays != null ? JsonSerializer.Serialize(dto.AvailableDays) : "[]",
                    PlanCategory = "booking",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _planBookingRepository.AddAsync(entity);
                return ApiResponse.SuccessResponse(MapToDto(entity), "Plan booking created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to create plan booking", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse> UpdatePlanBookingAsync(PlanBookingUpdateDto dto, List<IFormFile>? newImages)
        {
            try
            {
                var existing = await _planBookingRepository.GetByIdAsync(dto.RecId);
                if (existing == null) return ApiResponse.ErrorResponse("Plan booking not found", 404);

                if (newImages != null && newImages.Count > 0)
                {
                    var oldUrls = JsonSerializer.Deserialize<List<string>>(existing.Images ?? "[]") ?? new List<string>();
                    _fileService.DeleteImages(oldUrls);

                    var newUrls = await _fileService.SaveImagesAsync(newImages, "planbookings");
                    existing.Images = JsonSerializer.Serialize(newUrls);
                }

                existing.FkCompany = dto.FkCompany ?? existing.FkCompany;
                existing.FkWorkspace = dto.FkWorkspace;
                existing.FkWorkspaceType = dto.FkWorkspaceType;
                existing.FkLocation = dto.FkLocation;
                existing.Name = dto.Name;
                existing.Description = dto.Description;
                existing.StartTime = dto.StartTime;
                existing.EndTime = dto.EndTime;
                existing.MinDurationMinutes = dto.MinDurationMinutes;
                existing.MaxDurationMinutes = dto.MaxDurationMinutes;
                existing.PriceType = dto.PriceType;
                existing.Price = dto.Price;
                existing.AllowCancellation = dto.AllowCancellation;
                existing.RequiresApproval = dto.RequiresApproval;
                existing.Features = dto.Features != null ? JsonSerializer.Serialize(dto.Features) : existing.Features;
                existing.AvailableDays = dto.AvailableDays != null ? JsonSerializer.Serialize(dto.AvailableDays) : existing.AvailableDays;
                existing.UpdatedAt = DateTime.UtcNow;

                await _planBookingRepository.UpdateAsync(existing);
                return ApiResponse.SuccessResponse(MapToDto(existing), "Plan booking updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to update plan booking", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse> DeletePlanBookingAsync(Guid id)
        {
            try
            {
                var isDeleted = await _planBookingRepository.DeleteAsync(id);
                if (!isDeleted) return ApiResponse.ErrorResponse("Plan booking not found", 404);

                return ApiResponse.SuccessResponse(null, "Plan booking deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to delete plan booking", 500, new List<string> { ex.Message });
            }
        }

        private PlanBookingDto MapToDto(PlanBooking planBooking) => new PlanBookingDto
        {
            RecId = planBooking.RecId,
            FkCompany = planBooking.FkCompany,
            FkWorkspace = planBooking.FkWorkspace,
            FkWorkspaceType = planBooking.FkWorkspaceType,
            FkLocation = planBooking.FkLocation,
            Name = planBooking.Name!,
            Description = planBooking.Description,
            StartTime = planBooking.StartTime,
            EndTime = planBooking.EndTime,
            MinDurationMinutes = planBooking.MinDurationMinutes,
            MaxDurationMinutes = planBooking.MaxDurationMinutes,
            PriceType = planBooking.PriceType,
            Price = planBooking.Price,
            AllowCancellation = planBooking.AllowCancellation,
            RequiresApproval = planBooking.RequiresApproval,
            PlanCategory = planBooking.PlanCategory,
            IsActive = planBooking.IsActive,
            CreatedAt = planBooking.CreatedAt,
            UpdatedAt = planBooking.UpdatedAt,
            Images = JsonSerializer.Deserialize<List<string>>(planBooking.Images ?? "[]") ?? new(),
            Features = JsonSerializer.Deserialize<List<string>>(planBooking.Features ?? "[]") ?? new(),
            AvailableDays = JsonSerializer.Deserialize<List<string>>(planBooking.AvailableDays ?? "[]") ?? new()
        };
    }
}
