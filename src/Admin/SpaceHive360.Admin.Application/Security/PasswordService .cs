using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Application.Security
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<object> _hasher =
            new PasswordHasher<object>();

        public string Hash(string password)
        {
            return _hasher.HashPassword(null!, password);
        }

        public bool Verify(string hash, string password)
        {
            var result = _hasher.VerifyHashedPassword(
                null!,
                hash,
                password
            );

            return result == PasswordVerificationResult.Success;
        }
    }
}
