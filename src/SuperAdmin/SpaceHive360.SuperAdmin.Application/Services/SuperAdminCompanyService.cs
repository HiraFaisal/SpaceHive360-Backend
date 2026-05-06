using SpaceHive360.SuperAdmin.Application.DTOs;
using SpaceHive360.SuperAdmin.Application.Interfaces;
using SpaceHive360.SuperAdmin.Domain.IRepositories;
using SpaceHive360.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.SuperAdmin.Application.Services
{
    public class SuperAdminCompanyService : ISuperAdminCompanyService
    {
        private readonly ISuperAdminCompanyRepository _companyRepository;

        public SuperAdminCompanyService(ISuperAdminCompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<IEnumerable<SuperAdminCompanyResponseDto>> GetAllCompaniesAsync(string? status = null)
        {
            var companies = await _companyRepository.GetAllCompaniesAsync(status);
            return companies.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<SuperAdminCompanyResponseDto>> GetPendingCompaniesAsync()
        {
            var companies = await _companyRepository.GetPendingCompaniesAsync();
            return companies.Select(MapToResponseDto);
        }

        public async Task<bool> ApproveCompanyAsync(Guid id, string? remarks, Guid superAdminId)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null) return false;

            company.RegistrationStatus = "Approved";
            company.AdminComments = remarks;
            company.ApprovedBy = superAdminId;
            company.ApprovedAt = DateTime.UtcNow;
            company.IsActive = true;
            company.UpdatedAt = DateTime.UtcNow;

            await _companyRepository.UpdateAsync(company);
            return true;
        }

        public async Task<bool> RejectCompanyAsync(Guid id, string? remarks, Guid superAdminId)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null) return false;

            company.RegistrationStatus = "Rejected";
            company.AdminComments = remarks;
            company.ApprovedBy = superAdminId; // Reused field for "reviewed by"
            company.ApprovedAt = DateTime.UtcNow; // Reused field for "reviewed at"
            company.IsActive = false;
            company.UpdatedAt = DateTime.UtcNow;

            await _companyRepository.UpdateAsync(company);
            return true;
        }

        public async Task<bool> ReviewCompanyAsync(Guid id, string? remarks, Guid superAdminId)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null) return false;

            company.RegistrationStatus = "UnderReview";
            company.AdminComments = remarks;
            company.UpdatedAt = DateTime.UtcNow;

            await _companyRepository.UpdateAsync(company);
            return true;
        }

        private SuperAdminCompanyResponseDto MapToResponseDto(Company company)
        {
            return new SuperAdminCompanyResponseDto
            {
                CompanyId = company.RecId,
                CompanyName = company.Name,
                Email = company.Email,
                Status = company.RegistrationStatus,
                CreatedAt = company.CreatedAt,
                ReviewedBy = company.ApprovedBy,
                ReviewedAt = company.ApprovedAt,
                Remarks = company.AdminComments,
                ContactPersonName = company.ContactPersonName
            };
        }
    }
}
