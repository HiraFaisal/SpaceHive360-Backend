using SpaceHive360.Member.Application.Models;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Application.Services
{
    public interface IAuthService
    {
        Task<ApiResponse> LoginAsync(LoginRequest request);
        Task<ApiResponse> RegisterAsync(RegisterRequest request);
        Task<ApiResponse> GetProfileAsync(Guid userId);
    }
}
