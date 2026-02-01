using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Application.DTOs
{
    public class Login
    {
        public class LoginResponse
        {
            public string Token { get; set; } = null!;
            public DateTime Expiration { get; set; }
        }
        public class LoginRequest
        {
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
        }
    }
}
