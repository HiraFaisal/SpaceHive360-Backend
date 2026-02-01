using SpaceHive360.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static SpaceHive360.Admin.Application.DTOs.Login;

namespace SpaceHive360.Admin.Application.Security.JwtToken
{
    public interface IJwtTokenService
    {
        string GenerateToken(AdminUser user);
    }
}
