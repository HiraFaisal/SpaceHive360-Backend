using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Member.Domain.Entities
{
    [Table("tbl_user_preferences")]
    public class UserPreference
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("fk_member")]
        public Guid FkMember { get; set; }

        [Column("budget_min")]
        public decimal? BudgetMin { get; set; }

        [Column("budget_max")]
        public decimal? BudgetMax { get; set; }

        [Column("preferred_lat")]
        public double? PreferredLat { get; set; }

        [Column("preferred_lng")]
        public double? PreferredLng { get; set; }

        [Column("environment")]
        public string? Environment { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
