using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Member.Domain.Entities
{
    [Table("tbl_member_bookings")]
    public class MemberBooking
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("fk_member_user")]
        public Guid FkMemberUser { get; set; }

        [Column("fk_plan")]
        public Guid FkPlan { get; set; }

        [Column("plan_type")]
        public string PlanType { get; set; } = string.Empty; // 'Membership' or 'Booking'

        [Column("booking_date")]
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("total_amount")]
        public decimal? TotalAmount { get; set; }

        [Column("payment_status")]
        public string PaymentStatus { get; set; } = "Pending";

        [Column("booking_status")]
        public string BookingStatus { get; set; } = "Pending";

        [Column("payment_id")]
        public string? PaymentId { get; set; }

        [ForeignKey(nameof(FkMemberUser))]
        public virtual MemberUser? MemberUser { get; set; }
    }
}
