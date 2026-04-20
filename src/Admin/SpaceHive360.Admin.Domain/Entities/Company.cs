using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Admin.Domain.Entities
{
    [Table("tbl_companies")]
    public class Company
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("name")]
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [Column("email")]
        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Column("phone")]
        [MaxLength(30)]
        public string? Phone { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Column("logo_url")]
        public string? LogoUrl { get; set; }

        [Column("website")]
        public string? Website { get; set; }

        [Column("tax_id")]
        public string? TaxId { get; set; }

        [Column("contact_person")]
        public string? ContactPersonName { get; set; }

        [Column("registration_status")]
        public string RegistrationStatus { get; set; } = "Pending"; // Pending, Approved, Rejected

        [Column("admin_comments")]
        public string? AdminComments { get; set; }

        [Column("approved_by")]
        public Guid? ApprovedBy { get; set; }

        [Column("approved_at")]
        public DateTime? ApprovedAt { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
