using SpaceHive360.SuperAdmin.Domain.Entities;
using SpaceHive360.Admin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SpaceHive360.SuperAdmin.Infrastructure.Data
{
    public class SuperAdminDbContext : DbContext
    {
        public SuperAdminDbContext(DbContextOptions<SuperAdminDbContext> options)
            : base(options)
        {
        }

        public DbSet<SuperAdminUser> SuperAdminUsers { get; set; }
        public DbSet<Company> Companies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SuperAdminUser>().ToTable("tbl_super_admin_users");
            modelBuilder.Entity<Company>().ToTable("tbl_companies");

            base.OnModelCreating(modelBuilder);
        }
    }
}
