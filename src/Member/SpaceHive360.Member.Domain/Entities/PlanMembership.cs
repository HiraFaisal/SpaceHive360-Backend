using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Member.Domain.Entities
{
    [Table("tbl_plan_membership")]
    public class PlanMembership
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("fk_company")]
        public Guid? FkCompany { get; set; }

        [Column("fk_workspace_type")]
        public Guid? FkWorkspaceType { get; set; }

        [Column("fk_workspace")]
        public Guid? FkWorkspace { get; set; }

        [Column("fk_location")]
        public Guid? FkLocation { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("duration_type")]
        public string? DurationType { get; set; }

        [Column("duration_value")]
        public int? DurationValue { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        [Column("images")]
        public string? Images { get; set; }

        [Column("features")]
        public string? Features { get; set; }

        [Column("is_recurring")]
        public bool IsRecurring { get; set; } = false;

        [Column("plan_category")]
        public string? PlanCategory { get; set; } = "membership";

        [Column("isactive")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
