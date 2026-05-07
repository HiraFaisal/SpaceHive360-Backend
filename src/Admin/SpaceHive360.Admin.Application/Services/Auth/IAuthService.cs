using System;
using System.Collections.Generic;
using System.Text;
using static SpaceHive360.Admin.Application.DTOs.Login;

namespace SpaceHive360.Admin.Application.Services.Auth
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginRequest request);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> VerifyResetCodeAsync(string email, string code);
        Task<bool> ResetPasswordAsync(string email, string code, string newPassword);
    }
}
