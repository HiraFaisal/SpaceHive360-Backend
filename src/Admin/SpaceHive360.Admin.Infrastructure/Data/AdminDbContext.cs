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
        public DbSet<City> Cities { get; set; }
        public DbSet<Location> CompanyLocations { get; set; }
        public DbSet<PaymentTerm> PaymentTerms { get; set; }
        public DbSet<Plan> Plans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminUser>().ToTable("tbl_admin_user");
            modelBuilder.Entity<Company>().ToTable("tbl_companies");
            modelBuilder.Entity<WorkspaceType>().ToTable("tbl_workspace_types");
            modelBuilder.Entity<Workspace>().ToTable("tbl_workspaces");
            modelBuilder.Entity<City>().ToTable("tbl_city");
            modelBuilder.Entity<Location>().ToTable("tbl_company_locations");
            modelBuilder.Entity<PaymentTerm>().ToTable("tbl_payment_terms");
            modelBuilder.Entity<Plan>(entity =>
            {
                entity.ToTable("tbl_plans");

                entity.Property(p => p.ImagesJson)
                      .HasColumnName("images")
                      .HasColumnType("jsonb")
                      .HasConversion(
                          v => v ?? "[]",                    // C# → DB
                          v => v                             // DB → C#
                      );

                entity.Property(p => p.FeaturesJson)
                      .HasColumnName("features")
                      .HasColumnType("jsonb")
                      .HasConversion(
                          v => v ?? "[]",                    // C# → DB
                          v => v                             // DB → C#
                      );
            });
        }
    }
}
