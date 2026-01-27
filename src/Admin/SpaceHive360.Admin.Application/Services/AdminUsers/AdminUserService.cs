using SpaceHive360.Admin.Domain.IRepositories;
using SpaceHive360.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Application.Services.AdminUsers
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IAdminUserRepository _repo;

        public AdminUserService(IAdminUserRepository repo)
        {
            _repo = repo;
        }


        public async Task CreateAsync(AdminUser user)
        {
            await _repo.AddAsync(user);
        }
    }
    }
