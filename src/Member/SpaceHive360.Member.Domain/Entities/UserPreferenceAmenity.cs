using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Member.Domain.Entities
{
    [Table("tbl_user_preference_amenities")]
    public class UserPreferenceAmenity
    {
        [Column("fk_preference")]
        public Guid FkPreference { get; set; }

        [Column("amenity_name")]
        public string AmenityName { get; set; } = string.Empty;
    }
}
