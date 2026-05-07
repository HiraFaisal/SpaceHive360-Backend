using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Admin.Domain.Entities
{
    [Table("tbl_plan_booking")]
    public class PlanBooking
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        // 🔗 References
        [Column("fk_company")]
        public Guid? FkCompany { get; set; }

        [Column("fk_workspace")]
        public Guid? FkWorkspace { get; set; }

        [Column("fk_workspace_type")]
        public Guid? FkWorkspaceType { get; set; }

        [Column("fk_location")]
        public Guid? FkLocation { get; set; }

        // 🧾 Basic Info
        [Column("name")]
        public string? Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        // 📦 JSON Fields
        [Column("images")]
        public string? Images { get; set; }

        [Column("features")]
        public string? Features { get; set; }

        // 🏷️ Classification
        [Column("plan_category")]
        public string? PlanCategory { get; set; } = "booking";

        // ⏰ Time Window
        [Column("start_time")]
        public TimeSpan? StartTime { get; set; }

        [Column("end_time")]
        public TimeSpan? EndTime { get; set; }

        // 📅 Available Days (JSON)
        [Column("available_days")]
        public string? AvailableDays { get; set; }

        // ⏳ Duration Rules
        [Column("min_duration_minutes")]
        public int? MinDurationMinutes { get; set; }

        [Column("max_duration_minutes")]
        public int? MaxDurationMinutes { get; set; }

        // 💳 Pricing
        [Column("price_type")]
        public string? PriceType { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        // ⚙️ Rules
        [Column("allow_cancellation")]
        public bool AllowCancellation { get; set; } = false;

        [Column("requires_approval")]
        public bool RequiresApproval { get; set; } = false;

        // 🧠 Audit
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("is_visible")]
        public bool IsVisible { get; set; } = true;

        [Column("is_ai_updated")]
        public bool IsAiUpdated { get; set; } = false;
    }
}