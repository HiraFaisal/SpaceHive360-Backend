using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Admin.Domain.Entities
{
    [Table("tbl_member_memberships")]
    public class MemberMembership
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("fk_member_user")]
        public Guid FkMemberUser { get; set; }

        [Column("fk_plan")]
        public Guid FkPlan { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("membership_status")]
        public string MembershipStatus { get; set; } = "Pending";

        [Column("fk_payment")]
        public Guid? FkPayment { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
