using System;
using System.Collections.Generic;
using System.Text;
using static SpaceHive360.Admin.Application.DTOs.Login;

namespace SpaceHive360.Admin.Application.Services.Auth
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginRequest request);
    }
}
