using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Member.Domain.Entities
{
    [Table("tbl_member_payments", Schema = "public")]
    public class MemberPayment
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("fk_member_user")]
        public Guid FkMemberUser { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("payment_method")]
        public string PaymentMethod { get; set; } = string.Empty; // Stripe, BankTransfer

        [Column("payment_status")]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Completed, Failed

        [Column("approval_status")]
        public string ApprovalStatus { get; set; } = "Pending"; // Pending, Approved, Rejected

        [Column("transaction_id")]
        public string? TransactionId { get; set; } // Stripe Session Id or manual ref

        [Column("account_title")]
        public string? AccountTitle { get; set; }

        [Column("account_number")]
        public string? AccountNumber { get; set; }

        [Column("iban_number")]
        public string? IbanNumber { get; set; }

        [Column("bank_name")]
        public string? BankName { get; set; }

        [Column("payment_proof")]
        public string? PaymentProof { get; set; } // Maps to text in PG

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
