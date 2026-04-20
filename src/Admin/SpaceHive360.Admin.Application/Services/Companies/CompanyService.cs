using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Application.Models;
using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Application.Services.Companies
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<ApiResponse> RegisterCompanyAsync(CompanyRegistrationDto dto)
        {
            try
            {
                if (await _companyRepository.ExistsByEmailAsync(dto.Email))
                    return ApiResponse.ErrorResponse("A company with this email already exists.", 400);

                var company = new Company
                {
                    RecId = Guid.NewGuid(),
                    Name = dto.Name,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Address = dto.Address,
                    Website = dto.Website,
                    TaxId = dto.TaxId,
                    ContactPersonName = dto.ContactPersonName,
                    RegistrationStatus = "Pending",
                    IsActive = false, // Inactive until approved
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _companyRepository.AddAsync(company);
                return ApiResponse.SuccessResponse(MapToDto(company), "Company registration submitted for approval.");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to register company.", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse> GetCompanyStatusAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return ApiResponse.ErrorResponse("Email is required.", 400);

                var query = await _companyRepository.GetAllAsync();
                var company = query.FirstOrDefault(c => c.Email.ToLower() == email.ToLower());

                if (company == null)
                    return ApiResponse.ErrorResponse("No registration found for this email.", 404);

                return ApiResponse.SuccessResponse(new
                {
                    company.Name,
                    company.RegistrationStatus,
                    company.AdminComments,
                    company.CreatedAt
                }, "Status fetched successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to fetch status.", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse> GetCompanyByIdAsync(Guid id)
        {
            try
            {
                var company = await _companyRepository.GetByIdAsync(id);
                if (company == null) return ApiResponse.ErrorResponse("Company not found.", 404);
                return ApiResponse.SuccessResponse(MapToDto(company));
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to fetch company.", 500, new List<string> { ex.Message });
            }
        }

        private CompanyDto MapToDto(Company c) => new CompanyDto
        {
            RecId = c.RecId,
            Name = c.Name,
            Email = c.Email,
            Phone = c.Phone,
            Address = c.Address,
            LogoUrl = c.LogoUrl,
            Website = c.Website,
            TaxId = c.TaxId,
            ContactPersonName = c.ContactPersonName,
            RegistrationStatus = c.RegistrationStatus,
            AdminComments = c.AdminComments,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        };
    }
}
