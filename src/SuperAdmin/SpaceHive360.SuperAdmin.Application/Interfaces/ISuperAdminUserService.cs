using SpaceHive360.SuperAdmin.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceHive360.SuperAdmin.Application.Interfaces
{
    public interface ISuperAdminUserService
    {
        Task<SuperAdminUserResponseDto> CreateAsync(CreateSuperAdminRequest request);
        Task<IEnumerable<SuperAdminUserResponseDto>> GetAllAsync();
    }
}
