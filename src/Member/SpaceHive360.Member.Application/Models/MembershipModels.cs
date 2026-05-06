using Microsoft.AspNetCore.Http;
using System;

namespace SpaceHive360.Member.Application.Models
{
    public class MembershipPurchaseRequest
    {
        public Guid PlanId { get; set; }
        public Guid MemberUserId { get; set; }
        public DateTime StartDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // Stripe, BankTransfer
        
        // Bank Transfer Fields
        public string? AccountTitle { get; set; }
        public string? AccountNumber { get; set; }
        public string? IbanNumber { get; set; }
        public string? BankName { get; set; }
        public IFormFile? PaymentScreenshot { get; set; }

        // Optional URLs for Stripe
        public string? SuccessUrl { get; set; }
        public string? CancelUrl { get; set; }
    }

    public class BookingPurchaseRequest
    {
        public Guid PlanId { get; set; }
        public Guid MemberUserId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;

        // Scheduling
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsFullDay { get; set; }

        // Recurrence
        public bool IsRecurring { get; set; }
        public int? RecurrenceInterval { get; set; }
        public string? EndType { get; set; } // Never, After, On
        public int? EndAfterOccurrences { get; set; }
        public DateTime? RecurrenceEndDate { get; set; }
        public string? SelectedDays { get; set; }

        // Bank Transfer Fields
        public string? AccountTitle { get; set; }
        public string? AccountNumber { get; set; }
        public string? IbanNumber { get; set; }
        public string? BankName { get; set; }
        public IFormFile? PaymentScreenshot { get; set; }

        public string? SuccessUrl { get; set; }
        public string? CancelUrl { get; set; }
    }

    public class MyMembershipDto
    {
        public Guid RecId { get; set; }
        public string WorkspaceName { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int RemainingDays { get; set; }
        public decimal Amount { get; set; }
        public bool IsExtendable { get; set; }
        public bool IsRenewable { get; set; }
        public Guid PlanId { get; set; }
        public string? Image { get; set; }
    }
}
