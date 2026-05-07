using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Admin.Domain.Entities.Community
{
    [Table("tbl_community_post")]
    public class CommunityPost
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("fk_company")]
        public Guid? FkCompany { get; set; }

        [Column("author_name")]
        [Required]
        [MaxLength(255)]
        public string AuthorName { get; set; } = string.Empty;

        [Column("author_role")]
        [MaxLength(255)]
        public string AuthorRole { get; set; } = string.Empty;

        [Column("author_location")]
        [MaxLength(255)]
        public string AuthorLocation { get; set; } = string.Empty;

        [Column("author_initials")]
        [MaxLength(10)]
        public string AuthorInitials { get; set; } = string.Empty;

        [Column("content")]
        [Required]
        public string Content { get; set; } = string.Empty;

        [Column("tag")]
        [MaxLength(50)]
        public string Tag { get; set; } = "GENERAL";

        [Column("tag_color")]
        [MaxLength(100)]
        public string TagColor { get; set; } = string.Empty;

        [Column("has_image")]
        public bool HasImage { get; set; } = false;

        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [Column("likes_count")]
        public int LikesCount { get; set; } = 0;

        [Column("comments_count")]
        public int CommentsCount { get; set; } = 0;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
