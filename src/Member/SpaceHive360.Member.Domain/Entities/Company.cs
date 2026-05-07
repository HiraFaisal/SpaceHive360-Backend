using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Member.Domain.Entities
{
    [Table("tbl_companies")]
    public class Company
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("registration_status")]
        public string RegistrationStatus { get; set; } = "Approved";
    }
}
