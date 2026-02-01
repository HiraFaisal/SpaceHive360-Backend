using SpaceHive360.Admin.Application.Security;
using SpaceHive360.Admin.Domain.IRepositories;
using static SpaceHive360.Admin.Application.DTOs.Login;
using SpaceHive360.Admin.Application.Security.JwtToken;
namespace SpaceHive360.Admin.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAdminUserRepository _repository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtTokenService _jwtGenerator;

        public AuthService(
            IAdminUserRepository repository,
            IPasswordService passwordService,
            IJwtTokenService jwtGenerator)
        {
            _repository = repository;
            _passwordService = passwordService;
            _jwtGenerator = jwtGenerator;
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
    }
}
