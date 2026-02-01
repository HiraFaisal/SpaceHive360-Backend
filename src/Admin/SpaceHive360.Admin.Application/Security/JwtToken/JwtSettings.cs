using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Application.Security.JwtToken
{
    public class JwtSettings
    {
        public string Secret { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int ExpiryMinutes { get; set; }
    }
}
