using SpaceHive360.Admin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Infrastructure.Data
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options)
            : base(options)
        {
        }

        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<WorkspaceType> WorkspaceTypes { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }

        public DbSet<PaymentTerms> PaymentTerms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminUser>().ToTable("tbl_admin_user");
            modelBuilder.Entity<Company>().ToTable("tbl_companies");
            modelBuilder.Entity<WorkspaceType>().ToTable("tbl_workspace_types");
            modelBuilder.Entity<Workspace>().ToTable("tbl_workspaces");
            modelBuilder.Entity<PaymentTerms>().ToTable("tbl_payment_terms");
        }
    }
}
