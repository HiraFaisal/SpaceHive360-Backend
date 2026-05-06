using System;

namespace SpaceHive360.Member.Application.Models
{
    public class CreateCheckoutSessionRequest
    {
        public Guid PlanId { get; set; }
        public string PlanType { get; set; } = string.Empty; // 'Membership' or 'Booking'
        public string SuccessUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
        public Guid MemberUserId { get; set; }
        public Guid? MembershipId { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? PaymentId { get; set; }
    }

    public class CheckoutSessionResponse
    {
        public string SessionId { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string CheckoutUrl { get; set; } = string.Empty;
    }
}
