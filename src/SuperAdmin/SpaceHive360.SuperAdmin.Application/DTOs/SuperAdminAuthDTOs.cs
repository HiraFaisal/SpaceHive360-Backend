namespace SpaceHive360.SuperAdmin.Application.DTOs
{
    public class SuperAdminLoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class SuperAdminLoginResponse
    {
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
    }
}
