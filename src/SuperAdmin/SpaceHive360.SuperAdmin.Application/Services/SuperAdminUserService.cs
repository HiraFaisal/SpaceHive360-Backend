using SpaceHive360.SuperAdmin.Application.DTOs;
using SpaceHive360.SuperAdmin.Application.Interfaces;
using SpaceHive360.SuperAdmin.Application.Security;
using SpaceHive360.SuperAdmin.Domain.Entities;
using SpaceHive360.SuperAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.SuperAdmin.Application.Services
{
    public class SuperAdminUserService : ISuperAdminUserService
    {
        private readonly ISuperAdminUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public SuperAdminUserService(
            ISuperAdminUserRepository userRepository,
            IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        public async Task<SuperAdminUserResponseDto> CreateAsync(CreateSuperAdminRequest request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new Exception("User with this email already exists.");

            var user = new SuperAdminUser
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = _passwordService.Hash(request.Password),
                Role = Guid.Empty, // Default GUID for role since DB expects UUID
                Status = 1, // Default to Active (int)
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            return MapToResponseDto(user, request.FullName);
        }

        public async Task<IEnumerable<SuperAdminUserResponseDto>> GetAllAsync()
        {
            return new List<SuperAdminUserResponseDto>();
        }

        private SuperAdminUserResponseDto MapToResponseDto(SuperAdminUser user, string fullName = "")
        {
            return new SuperAdminUserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = fullName, // From request since not in DB
                Status = user.Status.ToString(),
                CreatedAt = user.CreatedAt
            };
        }
    }
}
