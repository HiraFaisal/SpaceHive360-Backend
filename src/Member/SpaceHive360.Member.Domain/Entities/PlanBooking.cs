using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Member.Domain.Entities
{
    [Table("tbl_plan_booking")]
    public class PlanBooking
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("fk_company")]
        public Guid? FkCompany { get; set; }

        [Column("fk_workspace")]
        public Guid? FkWorkspace { get; set; }

        [Column("fk_workspace_type")]
        public Guid? FkWorkspaceType { get; set; }

        [Column("fk_location")]
        public Guid? FkLocation { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("images")]
        public string? Images { get; set; }

        [Column("features")]
        public string? Features { get; set; }

        [Column("plan_category")]
        public string? PlanCategory { get; set; } = "booking";

        [Column("start_time")]
        public TimeSpan? StartTime { get; set; }

        [Column("end_time")]
        public TimeSpan? EndTime { get; set; }

        [Column("available_days")]
        public string? AvailableDays { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
