using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.SuperAdmin.Domain.Entities
{
    [Table("tbl_super_admin_users")]
    public class SuperAdminUser
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; } = null!;

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = null!;

        [Column("role")]
        public Guid Role { get; set; }

        [Column("status")]
        public int Status { get; set; } = 1; // 1 = Active, 0 = Inactive

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Note: FullName is NOT in the DB schema provided by user, 
        // but it was in the request DTO. I will remove it from the entity 
        // to avoid "Column not found" errors unless the user wants to add it to DB.
    }
}
