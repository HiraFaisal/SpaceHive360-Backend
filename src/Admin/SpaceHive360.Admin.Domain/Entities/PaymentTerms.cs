using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SpaceHive360.Admin.Domain.Entities
{
    [Table("tbl_payment_terms")]
    public class PaymentTerms
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("fk_company")]
        [Required]
        public Guid FkCompany { get; set; }

        [Column("name")]
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }

        [Column("billing_cycle_months")]
        [Required]
        public int BillingCycleMonths { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_by")]
        public Guid? CreatedBy { get; set; }

        [Column("updated_by")]
        public Guid? UpdatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
