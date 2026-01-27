using SpaceHive360.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Domain.IRepositories
{
    public interface IAdminUserRepository
    {
        Task AddAsync(AdminUser user);
    }
}
