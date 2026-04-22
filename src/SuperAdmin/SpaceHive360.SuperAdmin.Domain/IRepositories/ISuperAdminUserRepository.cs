using SpaceHive360.SuperAdmin.Domain.Entities;
using System.Threading.Tasks;

namespace SpaceHive360.SuperAdmin.Domain.IRepositories
{
    public interface ISuperAdminUserRepository
    {
        Task AddAsync(SuperAdminUser user);
        Task<SuperAdminUser?> GetByEmailAsync(string email);
        Task<SuperAdminUser?> GetByIdAsync(Guid id);
    }
}
