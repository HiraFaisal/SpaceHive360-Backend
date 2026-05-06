using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Admin.Domain.Entities
{
    [Table("tbl_workspaces")]
    public class Workspace
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("fk_company")]
        [Required]
        public Guid FkCompany { get; set; }

        [Column("fk_workspace_type")]
        [Required]
        public Guid FkWorkspaceType { get; set; }

        [Column("fk_location")]
        public Guid? FkLocation { get; set; }

        [Column("created_by")]
        public Guid? CreatedBy { get; set; }

        [Column("unique_code")]
        public string? UniqueCode { get; set; }

        [Column("name")]
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }

        [Column("capacity")]
        public int? Capacity { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("is_available")]
        public bool IsAvailable { get; set; } = true;

        [Column("inventory_type")]
        public string InventoryType { get; set; } = "UNIT";

        [Column("max_units")]
        public int MaxUnits { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
