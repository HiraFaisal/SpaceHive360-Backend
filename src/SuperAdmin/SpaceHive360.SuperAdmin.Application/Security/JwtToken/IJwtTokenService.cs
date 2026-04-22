using SpaceHive360.SuperAdmin.Domain.Entities;

namespace SpaceHive360.SuperAdmin.Application.Security.JwtToken
{
    public interface IJwtTokenService
    {
        string GenerateToken(SuperAdminUser user);
    }
}
