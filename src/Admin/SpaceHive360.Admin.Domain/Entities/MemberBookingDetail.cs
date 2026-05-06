using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Admin.Domain.Entities
{
    [Table("tbl_member_booking_details")]
    public class MemberBookingDetail
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("fk_booking")]
        public Guid FkBooking { get; set; }

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime EndTime { get; set; }

        [Column("is_full_day")]
        public bool IsFullDay { get; set; }

        [Column("is_recurring")]
        public bool IsRecurring { get; set; }

        [Column("recurrence_interval")]
        public int? RecurrenceInterval { get; set; }

        [Column("recurrence_type")]
        public string? RecurrenceType { get; set; }

        [Column("end_type")]
        public string? EndType { get; set; }

        [Column("end_after_occurrences")]
        public int? EndAfterOccurrences { get; set; }

        [Column("recurrence_end_date")]
        public DateTime? RecurrenceEndDate { get; set; }

        [Column("selected_days")]
        public string? SelectedDays { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
