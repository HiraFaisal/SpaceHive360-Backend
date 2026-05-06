using Microsoft.EntityFrameworkCore;
using SpaceHive360.Member.Application.Models;
using SpaceHive360.Member.Application.Services;
using SpaceHive360.Member.Domain.Entities;
using SpaceHive360.Member.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly MemberDbContext _context;

        public AuthService(MemberDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.MemberUsers
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || user.PasswordHash != request.Password) // Simplified for now
            {
                return ApiResponse.ErrorResponse("Invalid email or password");
            }

            return ApiResponse.SuccessResponse(new AuthResponse
            {
                UserId = user.RecId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsOnboardingCompleted = user.IsOnboardingCompleted,
                Token = $"{user.RecId}:mock-jwt-token-{Guid.NewGuid()}" // Include userId in token for identification
            });
        }

        public async Task<ApiResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _context.MemberUsers.AnyAsync(u => u.Email == request.Email))
            {
                return ApiResponse.ErrorResponse("Email already registered");
            }

            var user = new MemberUser
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = request.Password, // Simplified
                PhoneNumber = request.PhoneNumber
            };

            _context.MemberUsers.Add(user);
            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResponse(new AuthResponse
            {
                UserId = user.RecId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsOnboardingCompleted = user.IsOnboardingCompleted,
                Token = $"{user.RecId}:mock-jwt-token-{Guid.NewGuid()}"
            });
        }

        public async Task<ApiResponse> GetProfileAsync(Guid userId)
        {
            var user = await _context.MemberUsers
                .FirstOrDefaultAsync(u => u.RecId == userId);

            if (user == null)
            {
                return ApiResponse.ErrorResponse("User not found");
            }

            return ApiResponse.SuccessResponse(new
            {
                user.RecId,
                user.FullName,
                user.Email,
                user.PhoneNumber
            });
        }
    }
}
