using Microsoft.EntityFrameworkCore;
using SpaceHive360.Member.Application.Models;
using SpaceHive360.Member.Application.Services;
using SpaceHive360.Member.Domain.Entities;
using SpaceHive360.Member.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Infrastructure.Services
{
    public class UserPreferenceService : IUserPreferenceService
    {
        private readonly MemberDbContext _context;

        public UserPreferenceService(MemberDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> SavePreferencesAsync(Guid userId, UserPreferenceRequest request)
        {
            try
            {
                var user = await _context.MemberUsers.FirstOrDefaultAsync(u => u.RecId == userId);
                if (user == null)
                    return new ApiResponse { Success = false, Message = "User not found." };

                // 1. Create or Update Preference
                var preference = await _context.UserPreferences.FirstOrDefaultAsync(p => p.FkMember == userId);
                bool isNew = false;
                if (preference == null)
                {
                    preference = new UserPreference
                    {
                        RecId = Guid.NewGuid(), // Manual UUID generation as requested
                        FkMember = userId,
                        CreatedAt = DateTime.UtcNow
                    };
                    isNew = true;
                }

                preference.BudgetMin = request.BudgetMin;
                preference.BudgetMax = request.BudgetMax;
                preference.PreferredLat = request.PreferredLat;
                preference.PreferredLng = request.PreferredLng;
                preference.Environment = request.Environment;
                preference.UpdatedAt = DateTime.UtcNow;

                if (isNew)
                    _context.UserPreferences.Add(preference);
                else
                    _context.UserPreferences.Update(preference);

                // IMPORTANT: Save changes to establish the preference record before adding amenities
                await _context.SaveChangesAsync();

                // 2. Update Amenities (Remove old, add new)
                var existingAmenities = _context.UserPreferenceAmenities.Where(a => a.FkPreference == preference.RecId);
                _context.UserPreferenceAmenities.RemoveRange(existingAmenities);
                // Save removals
                await _context.SaveChangesAsync();

                if (request.Amenities != null && request.Amenities.Any())
                {
                    var newAmenities = request.Amenities.Select(a => new UserPreferenceAmenity
                    {
                        FkPreference = preference.RecId,
                        AmenityName = a
                    });
                    _context.UserPreferenceAmenities.AddRange(newAmenities);
                    // Save additions
                    await _context.SaveChangesAsync();
                }

                // 3. Mark Onboarding as Completed
                user.IsOnboardingCompleted = true;
                await _context.SaveChangesAsync();

                return new ApiResponse { Success = true, Message = "Preferences saved successfully." };
            }
            catch (Exception ex)
            {
                // Return detailed error for debugging as requested
                var innerMsg = ex.InnerException != null ? $" | Inner: {ex.InnerException.Message}" : "";
                return new ApiResponse { Success = false, Message = $"Error: {ex.Message}{innerMsg}" };
            }
        }

        public async Task<ApiResponse> GetPreferencesAsync(Guid userId)
        {
            try
            {
                var preference = await _context.UserPreferences
                    .FirstOrDefaultAsync(p => p.FkMember == userId);

                if (preference == null)
                    return new ApiResponse { Success = false, Message = "Preferences not found." };

                var amenities = await _context.UserPreferenceAmenities
                    .Where(a => a.FkPreference == preference.RecId)
                    .Select(a => a.AmenityName)
                    .ToListAsync();

                return new ApiResponse 
                { 
                    Success = true, 
                    Data = new 
                    {
                        preference.BudgetMin,
                        preference.BudgetMax,
                        preference.PreferredLat,
                        preference.PreferredLng,
                        preference.Environment,
                        Amenities = amenities
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = $"Error retrieving preferences: {ex.Message}" };
            }
        }
    }
}
