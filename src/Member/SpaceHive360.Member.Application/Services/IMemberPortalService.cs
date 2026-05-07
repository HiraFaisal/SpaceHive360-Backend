using SpaceHive360.Member.Application.Models;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Application.Services
{
    public interface IMemberPortalService
    {
        Task<ApiResponse> GetCitiesAsync();
        Task<ApiResponse> GetCategoriesAsync();
        Task<ApiResponse> GetTopPlansAsync();
        Task<ApiResponse> GetPlansAsync(string? city, string? category);
        Task<ApiResponse> GetPlanByIdAsync(Guid id);
        Task<ApiResponse> GetLocationsAsync();
        Task<ApiResponse> GetStatsAsync();
    }
}
