using Microsoft.EntityFrameworkCore;
using SpaceHive360.Member.Domain.Entities;

namespace SpaceHive360.Member.Infrastructure.Data
{
    public class MemberDbContext : DbContext
    {
        public MemberDbContext(DbContextOptions<MemberDbContext> options)
            : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<WorkspaceType> WorkspaceTypes { get; set; }
        public DbSet<PlanMembership> PlanMemberships { get; set; }
        public DbSet<PlanBooking> PlanBookings { get; set; }
        public DbSet<MemberUser> MemberUsers { get; set; }
        public DbSet<MemberBooking> MemberBookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().ToTable("tbl_city");
            modelBuilder.Entity<Location>().ToTable("tbl_location");
            modelBuilder.Entity<WorkspaceType>().ToTable("tbl_workspace_types");
            modelBuilder.Entity<PlanMembership>().ToTable("tbl_plan_membership");
            modelBuilder.Entity<PlanBooking>().ToTable("tbl_plan_booking");
            modelBuilder.Entity<MemberUser>().ToTable("tbl_member_user");
            modelBuilder.Entity<MemberBooking>().ToTable("tbl_member_bookings");
            
            // Map JSON fields if needed, but for Member read-only we can keep them as strings or handle in application layer.
            // For simplicity and matching Admin, we can just treat them as strings for now.
        }
    }
}
