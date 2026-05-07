namespace SpaceHive360.Admin.Application.DTOs
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = null!;
    }

    public class VerifyResetCodeRequest
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
