using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Member.Domain.Entities
{
    [Table("tbl_workspaces")]
    public class Workspace
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("capacity")]
        public int? Capacity { get; set; }

        [Column("inventory_type")]
        public string InventoryType { get; set; } = "UNIT";

        [Column("max_units")]
        public int MaxUnits { get; set; } = 1;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("is_available")]
        public bool IsAvailable { get; set; } = true;
    }
}
