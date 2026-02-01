using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Application.Security
{
    public interface IPasswordService
    {
        string Hash(string password);
        bool Verify(string hash, string password);
    }
}
