using Microsoft.EntityFrameworkCore;
using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Domain.IRepositories;
using SpaceHive360.Admin.Infrastructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.Admin.Infrastructure.Repositories
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly AdminDbContext _context;

        public PasswordResetTokenRepository(AdminDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PasswordResetToken token)
        {
            await _context.PasswordResetTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<PasswordResetToken?> GetByEmailAndTokenAsync(string email, string token)
        {
            return await _context.PasswordResetTokens
                .Where(t => t.Email == email && t.Token == token && !t.IsUsed && t.ExpiryTime > DateTime.UtcNow)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(PasswordResetToken token)
        {
            _context.PasswordResetTokens.Update(token);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteExpiredTokensAsync()
        {
            var expired = _context.PasswordResetTokens.Where(t => t.ExpiryTime <= DateTime.UtcNow || t.IsUsed);
            _context.PasswordResetTokens.RemoveRange(expired);
            await _context.SaveChangesAsync();
        }
    }
}
