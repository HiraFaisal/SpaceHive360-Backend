using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceHive360.Admin.Domain.Entities
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
        public string PaymentMethod { get; set; } = string.Empty;

        [Column("payment_status")]
        public string PaymentStatus { get; set; } = "Pending";

        [Column("approval_status")]
        public string ApprovalStatus { get; set; } = "Pending";

        [Column("transaction_id")]
        public string? TransactionId { get; set; }

        [Column("account_title")]
        public string? AccountTitle { get; set; }

        [Column("account_number")]
        public string? AccountNumber { get; set; }

        [Column("iban_number")]
        public string? IbanNumber { get; set; }

        [Column("bank_name")]
        public string? BankName { get; set; }

        [Column("payment_proof")]
        public string? PaymentProof { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
