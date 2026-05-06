using SpaceHive360.Member.Application.Models;
using System;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Application.Services
{
    public interface IUserPreferenceService
    {
        Task<ApiResponse> SavePreferencesAsync(Guid userId, UserPreferenceRequest request);
        Task<ApiResponse> GetPreferencesAsync(Guid userId);
    }
}
