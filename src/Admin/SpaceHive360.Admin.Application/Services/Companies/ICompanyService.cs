using SpaceHive360.Admin.Application.Models;
using SpaceHive360.Admin.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Application.Services.Companies
{
    public interface ICompanyService
    {
        Task<ApiResponse> RegisterCompanyAsync(CompanyRegistrationDto dto);
        Task<ApiResponse> GetCompanyStatusAsync(string email);
        Task<ApiResponse> GetCompanyByIdAsync(Guid id);
    }
}
