using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SpaceHive360.Admin.Domain.Entities
{
    [Table("tbl_admin_user")]
    public class AdminUser
    {
        [Key]
        [Column("rec_id")]  
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("first_name")]
        [Required]
        public string FirstName { get; set; } = null!;

        [Column("last_name")]
        [Required]
        public string LastName { get; set; } = null!;

        [Column("email")]
        [Required]
        public string Email { get; set; } = null!;

        [Column("password")]
        [Required]
        public string Password { get; set; } = null!;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
