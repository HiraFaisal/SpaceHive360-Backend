using Microsoft.EntityFrameworkCore;
using SpaceHive360.SuperAdmin.Domain.Entities;
using SpaceHive360.SuperAdmin.Domain.IRepositories;
using SpaceHive360.SuperAdmin.Infrastructure.Data;
using System.Threading.Tasks;

namespace SpaceHive360.SuperAdmin.Infrastructure.Repositories
{
    public class SuperAdminUserRepository : ISuperAdminUserRepository
    {
        private readonly SuperAdminDbContext _context;

        public SuperAdminUserRepository(SuperAdminDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SuperAdminUser user)
        {
            await _context.SuperAdminUsers.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<SuperAdminUser?> GetByEmailAsync(string email)
        {
            return await _context.SuperAdminUsers.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<SuperAdminUser?> GetByIdAsync(Guid id)
        {
            return await _context.SuperAdminUsers.FindAsync(id);
        }
    }
}
