using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Admin.Domain.Entities
{
    [Table("tbl_password_reset_tokens")]
    public class PasswordResetToken
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("email")]
        [Required]
        public string Email { get; set; } = null!;

        [Column("token")]
        [Required]
        public string Token { get; set; } = null!;

        [Column("expiry_time")]
        public DateTime ExpiryTime { get; set; }

        [Column("is_used")]
        public bool IsUsed { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
