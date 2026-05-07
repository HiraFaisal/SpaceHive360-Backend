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
        public DbSet<FaqCategory> FaqCategories { get; set; }
        public DbSet<Faq> Faqs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SuperAdminUser>().ToTable("tbl_super_admin_users");
            modelBuilder.Entity<Company>().ToTable("tbl_companies");
            
            modelBuilder.Entity<FaqCategory>(entity =>
            {
                entity.ToTable("tbl_faq_category");
                entity.HasKey(e => e.RecId);
                entity.Property(e => e.RecId).HasColumnName("rec_id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
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
                    .HasForeignKey(d => d.FkCategory)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
