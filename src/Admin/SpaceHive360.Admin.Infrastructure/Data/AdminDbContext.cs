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
        //public DbSet<PaymentTerm> PaymentTerms { get; set; }
        public DbSet<Plan> Plans { get; set; }

        public DbSet<PaymentTerms> PaymentTerms { get; set; }

        public DbSet<Feedback> Feedbacks { get; set; }

        public DbSet<PlanMembership> PlanMemberships { get; set; }
        public DbSet<PlanBooking> PlanBookings { get; set; }
        public DbSet<MemberUser> MemberUsers { get; set; }
        public DbSet<MemberBooking> MemberBookings { get; set; }
        public DbSet<MemberBookingDetail> BookingDetails { get; set; }
        public DbSet<MemberMembership> MemberMemberships { get; set; }
        public DbSet<MemberPayment> MemberPayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminUser>().ToTable("tbl_admin_user");
            modelBuilder.Entity<Company>().ToTable("tbl_companies");
            modelBuilder.Entity<WorkspaceType>().ToTable("tbl_workspace_types");
            modelBuilder.Entity<Workspace>().ToTable("tbl_workspaces");
            modelBuilder.Entity<PaymentTerms>().ToTable("tbl_payment_terms");
            modelBuilder.Entity<City>().ToTable("tbl_city");
            modelBuilder.Entity<Location>().ToTable("tbl_location");
            modelBuilder.Entity<Feedback>().ToTable("tbl_feedback");
            modelBuilder.Entity<MemberUser>().ToTable("tbl_member_user");
            modelBuilder.Entity<MemberBooking>().ToTable("tbl_member_bookings");
            modelBuilder.Entity<MemberBookingDetail>().ToTable("tbl_member_booking_details");
            modelBuilder.Entity<MemberMembership>().ToTable("tbl_member_memberships");
            // ✅ Membership Plan Mapping
            modelBuilder.Entity<PlanMembership>(entity =>
            {
                entity.ToTable("tbl_plan_membership");

                entity.Property(p => p.Images)
                      .HasColumnName("images")
                      .HasColumnType("jsonb")
                      .HasConversion(
                          v => v ?? "[]",
                          v => v
                      );

                entity.Property(p => p.Features)
                      .HasColumnName("features")
                      .HasColumnType("jsonb")
                      .HasConversion(
                          v => v ?? "[]",
                          v => v
                      );
            });

            // ✅ Booking Plan Mapping
            modelBuilder.Entity<PlanBooking>(entity =>
            {
                entity.ToTable("tbl_plan_booking");

                entity.Property(p => p.Images)
                      .HasColumnName("images")
                      .HasColumnType("jsonb")
                      .HasConversion(
                          v => v ?? "[]",
                          v => v
                      );

                entity.Property(p => p.Features)
                      .HasColumnName("features")
                      .HasColumnType("jsonb")
                      .HasConversion(
                          v => v ?? "[]",
                          v => v
                      );

                entity.Property(p => p.AvailableDays)
                      .HasColumnName("available_days")
                      .HasColumnType("jsonb")
                      .HasConversion(
                          v => v ?? "[]",
                          v => v
                      );
            });
        }
    }
}
