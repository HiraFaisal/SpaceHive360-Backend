using SpaceHive360.SuperAdmin.Application.DTOs;
using System.Threading.Tasks;

namespace SpaceHive360.SuperAdmin.Application.Interfaces
{
    public interface ISuperAdminAuthService
    {
        Task<SuperAdminLoginResponse> LoginAsync(SuperAdminLoginRequest request);
    }
}
