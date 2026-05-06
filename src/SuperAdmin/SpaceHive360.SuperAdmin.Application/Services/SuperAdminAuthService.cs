using SpaceHive360.SuperAdmin.Application.DTOs;
using SpaceHive360.SuperAdmin.Application.Interfaces;
using SpaceHive360.SuperAdmin.Application.Security;
using SpaceHive360.SuperAdmin.Application.Security.JwtToken;
using SpaceHive360.SuperAdmin.Domain.IRepositories;
using System;
using System.Threading.Tasks;

namespace SpaceHive360.SuperAdmin.Application.Services
{
    public class SuperAdminAuthService : ISuperAdminAuthService
    {
        private readonly ISuperAdminUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtTokenService _jwtTokenService;

        public SuperAdminAuthService(
            ISuperAdminUserRepository userRepository,
            IPasswordService passwordService,
            IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<SuperAdminLoginResponse> LoginAsync(SuperAdminLoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            
            if (user == null || user.Status != 1)
                throw new Exception("Invalid credentials or account inactive.");

            if (!_passwordService.Verify(user.PasswordHash, request.Password))
                throw new Exception("Invalid credentials.");

            var token = _jwtTokenService.GenerateToken(user);

            return new SuperAdminLoginResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1) // Should ideally match JwtTokenService config
            };
        }
    }
}
