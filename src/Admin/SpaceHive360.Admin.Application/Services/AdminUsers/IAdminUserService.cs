using System;
using System.Collections.Generic;
using System.Text;
using SpaceHive360.Admin.Domain.Entities;

namespace SpaceHive360.Admin.Application.Services.AdminUsers
{
    public interface IAdminUserService
    {
        Task CreateAsync(AdminUser user);
    }
}
