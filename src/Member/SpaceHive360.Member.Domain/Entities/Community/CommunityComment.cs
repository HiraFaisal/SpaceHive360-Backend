using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Member.Domain.Entities.Community
{
    [Table("tbl_community_comment")]
    public class CommunityComment
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("fk_post")]
        public Guid FkPost { get; set; }

        [Column("member_name")]
        [Required]
        [MaxLength(255)]
        public string MemberName { get; set; } = string.Empty;

        [Column("member_initials")]
        [MaxLength(10)]
        public string MemberInitials { get; set; } = string.Empty;

        [Column("content")]
        [Required]
        public string Content { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
