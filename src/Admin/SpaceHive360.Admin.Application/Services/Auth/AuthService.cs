using SpaceHive360.Admin.Application.Security;
using SpaceHive360.Admin.Domain.IRepositories;
using static SpaceHive360.Admin.Application.DTOs.Login;
using SpaceHive360.Admin.Application.Security.JwtToken;
using SpaceHive360.Admin.Application.Services.Emails;
namespace SpaceHive360.Admin.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAdminUserRepository _repository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtTokenService _jwtGenerator;
        private readonly IPasswordResetTokenRepository _tokenRepository;
        private readonly IEmailService _emailService;

        public AuthService(
            IAdminUserRepository repository,
            IPasswordService passwordService,
            IJwtTokenService jwtGenerator,
            IPasswordResetTokenRepository tokenRepository,
            IEmailService emailService)
        {
            _repository = repository;
            _passwordService = passwordService;
            _jwtGenerator = jwtGenerator;
            _tokenRepository = tokenRepository;
            _emailService = emailService;
        }

        public async Task<string> LoginAsync(LoginRequest request)
        {
            var user = await _repository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new Exception("Invalid credentials");

            var validPassword = _passwordService.Verify(user.Password, request.Password);
            if (!validPassword)
                throw new Exception("Invalid credentials");

            return _jwtGenerator.GenerateToken(user);
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _repository.GetByEmailAsync(email);
            if (user == null) return false; // Don't reveal user existence? Actually, standard is to return true and say "If email exists..." but for admin we can be more direct or stick to standard.

            // Generate a 6-digit code
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();

            var token = new Domain.Entities.PasswordResetToken
            {
                Email = email,
                Token = code,
                ExpiryTime = DateTime.UtcNow.AddMinutes(15)
            };

            await _tokenRepository.AddAsync(token);

            // Send email
            var subject = "Password Reset - SpaceHive360";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px;'>
                    <h2 style='color: #0077C0; text-align: center;'>SpaceHive360 Admin</h2>
                    <p>Hello,</p>
                    <p>You requested to reset your password. Use the code below to proceed:</p>
                    <div style='background-color: #f4f4f4; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 5px; color: #333; border-radius: 5px; margin: 20px 0;'>
                        {code}
                    </div>
                    <p>This code will expire in 15 minutes.</p>
                    <p>If you did not request this, please ignore this email.</p>
                    <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;' />
                    <p style='font-size: 12px; color: #888; text-align: center;'>SpaceHive360 Management Dashboard</p>
                </div>";

            await _emailService.SendEmailAsync(email, subject, body);

            return true;
        }

        public async Task<bool> VerifyResetCodeAsync(string email, string code)
        {
            var token = await _tokenRepository.GetByEmailAndTokenAsync(email, code);
            return token != null;
        }

        public async Task<bool> ResetPasswordAsync(string email, string code, string newPassword)
        {
            var token = await _tokenRepository.GetByEmailAndTokenAsync(email, code);
            if (token == null) throw new Exception("Invalid or expired reset code");

            var user = await _repository.GetByEmailAsync(email);
            if (user == null) throw new Exception("User not found");

            // Update password
            user.Password = _passwordService.Hash(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(user);

            // Mark token as used
            token.IsUsed = true;
            await _tokenRepository.UpdateAsync(token);

            return true;
        }
    }
}
