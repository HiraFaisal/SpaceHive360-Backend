using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Admin.Application.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using static SpaceHive360.Admin.Application.DTOs.Login;
using SpaceHive360.Admin.Application.DTOs;

namespace SpaceHive360.Admin.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(new { token = result });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            try
            {
                await _authService.ForgotPasswordAsync(request.Email);
                return Ok(new { message = "If the email exists, a reset code has been sent." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode(VerifyResetCodeRequest request)
        {
            try
            {
                var isValid = await _authService.VerifyResetCodeAsync(request.Email, request.Code);
                if (isValid)
                    return Ok(new { message = "Reset code is valid." });
                
                return BadRequest(new { message = "Invalid or expired reset code." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            try
            {
                await _authService.ResetPasswordAsync(request.Email, request.Code, request.NewPassword);
                return Ok(new { message = "Password has been successfully reset." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
