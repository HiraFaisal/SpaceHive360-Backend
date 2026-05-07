using Microsoft.EntityFrameworkCore;
using SpaceHive360.Member.Domain.Entities;
using SpaceHive360.Member.Domain.Entities.Community;

namespace SpaceHive360.Member.Infrastructure.Data
{
    public class MemberDbContext : DbContext
    {
        public MemberDbContext(DbContextOptions<MemberDbContext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<WorkspaceType> WorkspaceTypes { get; set; }
        public DbSet<PlanMembership> PlanMemberships { get; set; }
        public DbSet<PlanBooking> PlanBookings { get; set; }
        public DbSet<MemberUser> MemberUsers { get; set; }
        public DbSet<MemberBooking> MemberBookings { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<UserPreferenceAmenity> UserPreferenceAmenities { get; set; }
        public DbSet<MemberMembership> MemberMemberships { get; set; }
        public DbSet<MemberPayment> MemberPayments { get; set; }
        public DbSet<MemberBookingDetail> BookingDetails { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }

        // Community
        public DbSet<CommunityPost> CommunityPosts { get; set; }
        public DbSet<CommunityComment> CommunityComments { get; set; }
        public DbSet<CommunityLike> CommunityLikes { get; set; }
        public DbSet<CommunityEvent> CommunityEvents { get; set; }

        public DbSet<UserActivity> UserActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().ToTable("tbl_city");
            modelBuilder.Entity<Location>().ToTable("tbl_location");
            modelBuilder.Entity<WorkspaceType>().ToTable("tbl_workspace_types");
            modelBuilder.Entity<PlanMembership>().ToTable("tbl_plan_membership");
            modelBuilder.Entity<PlanBooking>().ToTable("tbl_plan_booking");
            modelBuilder.Entity<MemberUser>().ToTable("tbl_member_user");
            modelBuilder.Entity<MemberBooking>().ToTable("tbl_member_bookings");
            modelBuilder.Entity<Feedback>().ToTable("tbl_feedback");
            modelBuilder.Entity<UserPreference>().ToTable("tbl_user_preferences");
            modelBuilder.Entity<UserPreferenceAmenity>().ToTable("tbl_user_preference_amenities");
            modelBuilder.Entity<UserActivity>().ToTable("tbl_user_activity");
            modelBuilder.Entity<MemberMembership>().ToTable("tbl_member_memberships");
            modelBuilder.Entity<MemberPayment>().ToTable("tbl_member_payments");

            // Community Mapping
            modelBuilder.Entity<CommunityPost>().ToTable("tbl_community_post");
            modelBuilder.Entity<CommunityComment>().ToTable("tbl_community_comment");
            modelBuilder.Entity<CommunityLike>().ToTable("tbl_community_like");
            modelBuilder.Entity<CommunityEvent>().ToTable("tbl_community_event");

            modelBuilder.Entity<UserPreferenceAmenity>()
                .HasKey(upa => new { upa.FkPreference, upa.AmenityName });
            
            // Map JSON fields if needed, but for Member read-only we can keep them as strings or handle in application layer.
            // For simplicity and matching Admin, we can just treat them as strings for now.
        }
    }
}
