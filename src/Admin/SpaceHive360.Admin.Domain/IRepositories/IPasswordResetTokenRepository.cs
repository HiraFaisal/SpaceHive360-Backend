using System;
using System.Threading.Tasks;
using SpaceHive360.Admin.Domain.Entities;

namespace SpaceHive360.Admin.Domain.IRepositories
{
    public interface IPasswordResetTokenRepository
    {
        Task AddAsync(PasswordResetToken token);
        Task<PasswordResetToken?> GetByEmailAndTokenAsync(string email, string token);
        Task UpdateAsync(PasswordResetToken token);
        Task DeleteExpiredTokensAsync();
    }
}
