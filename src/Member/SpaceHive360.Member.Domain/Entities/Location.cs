using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Member.Domain.Entities
{
    [Table("tbl_location")]
    public class Location
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("fk_company")]
        public Guid FkCompany { get; set; }

        [Column("fk_city")]
        public Guid FkCity { get; set; }

        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("address")]
        public string? Address { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;
    }
}
