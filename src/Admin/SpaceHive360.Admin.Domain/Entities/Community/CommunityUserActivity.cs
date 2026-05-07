using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Admin.Domain.Entities.Community
{
    [Table("tbl_user_activity")]
    public class CommunityUserActivity
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("fk_member")]
        public Guid FkMember { get; set; }

        [Column("workspace_id")]
        public string WorkspaceId { get; set; } = string.Empty;

        [Column("action_type")]
        public string ActionType { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
