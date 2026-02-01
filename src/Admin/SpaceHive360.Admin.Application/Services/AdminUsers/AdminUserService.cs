using SpaceHive360.Admin.Application.Security;
using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Application.Services.AdminUsers
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IAdminUserRepository _repo;
        private readonly IPasswordService _passwordService;
        public AdminUserService(IAdminUserRepository repo, IPasswordService passwordService)
        {
            _repo = repo;
            _passwordService = passwordService;
        }


        public async Task CreateAsync(AdminUser user)
        {
            var passwordHashed = _passwordService.Hash(user.Password);
            user.Password = passwordHashed;
            await _repo.AddAsync(user);
        }
        public async Task<bool> LoginAsync(string email, string password)
        {
            // 1️⃣ Find user by email
            var user = await _repo.GetByEmailAsync(email);
            if (user == null)
                return false;

            // 2️⃣ Verify password
            var isValid = _passwordService.Verify(user.Password, password);

            return isValid;
        }

    }
}
