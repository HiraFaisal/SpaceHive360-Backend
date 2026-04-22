using SpaceHive360.SuperAdmin.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceHive360.SuperAdmin.Application.Interfaces
{
    public interface ISuperAdminCompanyService
    {
        Task<IEnumerable<SuperAdminCompanyResponseDto>> GetAllCompaniesAsync(string? status = null);
        Task<IEnumerable<SuperAdminCompanyResponseDto>> GetPendingCompaniesAsync();
        Task<bool> ApproveCompanyAsync(Guid id, string? remarks, Guid superAdminId);
        Task<bool> RejectCompanyAsync(Guid id, string? remarks, Guid superAdminId);
        Task<bool> ReviewCompanyAsync(Guid id, string? remarks, Guid superAdminId);
    }
}
