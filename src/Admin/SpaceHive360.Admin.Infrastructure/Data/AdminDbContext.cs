using SpaceHive360.Admin.Domain.Entities;
using SpaceHive360.Admin.Domain.Entities.Community;
using SpaceHive360.SuperAdmin.Domain.Entities;
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
        
        // Community
        public DbSet<CommunityPost> CommunityPosts { get; set; }
        public DbSet<CommunityComment> CommunityComments { get; set; }
        public DbSet<CommunityLike> CommunityLikes { get; set; }
        public DbSet<CommunityEvent> CommunityEvents { get; set; }
        public DbSet<CommunityUserActivity> UserActivities { get; set; }

        // Support / FAQ
        public DbSet<FaqCategory> FaqCategories { get; set; }
        public DbSet<Faq> Faqs { get; set; }

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
            
            // Support / FAQ Mapping
            modelBuilder.Entity<FaqCategory>(entity =>
            {
                entity.ToTable("tbl_faq_category");
                entity.HasKey(e => e.RecId);
                entity.Property(e => e.RecId).HasColumnName("rec_id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Icon).HasColumnName("icon");
                entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<Faq>(entity =>
            {
                entity.ToTable("tbl_faq");
                entity.HasKey(e => e.RecId);
                entity.Property(e => e.RecId).HasColumnName("rec_id");
                entity.Property(e => e.FkCategory).HasColumnName("fk_category");
                entity.Property(e => e.Question).HasColumnName("question");
                entity.Property(e => e.Answer).HasColumnName("answer");
                entity.Property(e => e.Steps).HasColumnName("steps");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.CreatedBy).HasColumnName("created_by");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Faqs)
                    .HasForeignKey(d => d.FkCategory);
            });

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

            // Community Mapping
            modelBuilder.Entity<CommunityPost>().ToTable("tbl_community_post");
            modelBuilder.Entity<CommunityComment>().ToTable("tbl_community_comment");
            modelBuilder.Entity<CommunityLike>().ToTable("tbl_community_like");
            modelBuilder.Entity<CommunityEvent>().ToTable("tbl_community_event");
            modelBuilder.Entity<CommunityUserActivity>().ToTable("tbl_user_activity");
        }
    }
}
