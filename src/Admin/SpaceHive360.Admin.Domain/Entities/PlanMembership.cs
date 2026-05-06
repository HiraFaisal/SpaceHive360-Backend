using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Admin.Domain.Entities
{
    [Table("tbl_plan_membership")]
    public class PlanMembership
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        // 🔗 References
        [Column("fk_company")]
        public Guid? FkCompany { get; set; }

        [Column("fk_workspace_type")]
        public Guid? FkWorkspaceType { get; set; }

        [Column("fk_workspace")]
        public Guid? FkWorkspace { get; set; }

        [Column("fk_location")]
        public Guid? FkLocation { get; set; }

        // 🧾 Basic Info
        [Column("name")]
        public string? Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        // ⏳ Duration
        [Column("duration_type")]
        public string? DurationType { get; set; }

        [Column("duration_value")]
        public int? DurationValue { get; set; }

        // 💳 Payment
        [Column("fk_payment_term")]
        public string? FkPaymentTerm { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        // 📦 JSON Fields
        [Column("images")]
        public string? Images { get; set; }

        [Column("features")]
        public string? Features { get; set; }

        // 🔁 Rules
        [Column("is_recurring")]
        public bool IsRecurring { get; set; } = false;

        [Column("allow_cancellation")]
        public bool AllowCancellation { get; set; } = false;

        [Column("requires_approval")]
        public bool RequiresApproval { get; set; } = false;

        [Column("max_slots")]
        public int MaxSlots { get; set; } = 0;

        // 🏷️ Classification
        [Column("plan_category")]
        public string? PlanCategory { get; set; } = "membership";

        // 🧠 Audit
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("isactive")]
        public bool IsActive { get; set; } = true;
    }
}