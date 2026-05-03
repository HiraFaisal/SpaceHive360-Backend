using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SpaceHive360.Admin.Domain.Entities
{
    [Table("tbl_feedback")]
    public class Feedback
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("fk_company")]
        public Guid FkCompany { get; set; }

        [Column("fk_location")]
        public Guid? FkLocation { get; set; }

        [Column("fk_payment")]
        public Guid? FkPayment { get; set; }

        [Column("member_name")]
        public string? MemberName { get; set; }

        [Column("member_email")]
        public string? MemberEmail { get; set; }

        [Column("category")]
        public string? Category { get; set; }

        [Column("experience")]
        public string? Experience { get; set; }

        [Column("comments")]
        public string Comments { get; set; } = null!;

        [Column("rating")]
        public int? Rating { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("plan_booking_id")]
        public Guid? PlanBookingId { get; set; }

        [Column("plan_membership_id")]
        public Guid? PlanMembershipId { get; set; }

        [Column("sentiment")]
        public string? Sentiment { get; set; }

        [Column("sentiment_score")]
        public double? SentimentScore { get; set; }
    }
}
