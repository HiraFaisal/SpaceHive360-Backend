using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.SuperAdmin.Domain.Entities
{
    [Table("tbl_faq_category")]
    public class FaqCategory
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; }

        [Column("name")]
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column("icon")]
        [MaxLength(50)]
        public string Icon { get; set; } = "HelpCircle";

        [Column("display_order")]
        public int DisplayOrder { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Faq> Faqs { get; set; } = new List<Faq>();
    }

    [Table("tbl_faq")]
    public class Faq
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; }

        [Column("fk_category")]
        public Guid FkCategory { get; set; }

        [Column("question")]
        [Required]
        public string Question { get; set; } = string.Empty;

        [Column("answer")]
        [Required]
        public string Answer { get; set; } = string.Empty;

        [Column("steps")]
        public string? Steps { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_by")]
        public Guid? CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("FkCategory")]
        public virtual FaqCategory Category { get; set; } = null!;
    }
}
