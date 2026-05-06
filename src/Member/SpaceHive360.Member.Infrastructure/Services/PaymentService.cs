using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using SpaceHive360.Member.Application.Models;
using SpaceHive360.Member.Application.Services;
using SpaceHive360.Member.Domain.Entities;
using SpaceHive360.Member.Infrastructure.Data;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly MemberDbContext _context;
        private readonly string _stripeSecretKey;
        private readonly string _webhookSecret;
        private readonly IInventoryService _inventoryService;

        public PaymentService(IConfiguration configuration, MemberDbContext context, IInventoryService inventoryService)
        {
            _configuration = configuration;
            _context = context;
            _inventoryService = inventoryService;
            _stripeSecretKey = _configuration["Stripe:SecretKey"] ?? "";
            _webhookSecret = _configuration["Stripe:WebhookSecret"] ?? "";
            StripeConfiguration.ApiKey = _stripeSecretKey;
        }

        public async Task<ApiResponse> CreateCheckoutSessionAsync(CreateCheckoutSessionRequest request)
        {
            try
            {
                // 1. Fetch Plan Details
                string planName = "";
                decimal price = 0;

                if (request.PlanType == "Membership")
                {
                    var plan = await _context.PlanMemberships.FindAsync(request.PlanId);
                    if (plan == null) return ApiResponse.ErrorResponse("Plan not found");
                    planName = plan.Name ?? "Membership Plan";
                    price = plan.Price ?? 0;
                }
                else
                {
                    var plan = await _context.PlanBookings.FindAsync(request.PlanId);
                    if (plan == null) return ApiResponse.ErrorResponse("Plan not found");
                    planName = plan.Name ?? "Booking Plan";
                    price = plan.Price ?? 0;
                }

                // 2. Determine Amount (Use request.Amount if provided, else fallback to plan price)
                decimal totalAmount = request.Amount ?? price;
                if (totalAmount <= 0) return ApiResponse.ErrorResponse("Invalid payment amount");

                // Stripe Minimum Amount Validation (Approx 50 cents USD)
                // For PKR, Stripe requires at least ~140-150 PKR. 
                if (totalAmount < 150)
                {
                    return ApiResponse.ErrorResponse(
                        $"The total amount (Rs. {totalAmount:F2}) is too low for Stripe processing. " +
                        "Stripe requires a minimum transaction of approximately Rs. 150 (equivalent to $0.50 USD). " +
                        "Please increase the booking duration or check the plan pricing.", 
                        400);
                }

                // 3. Update Existing Booking with Payment ID if provided
                if (request.BookingId.HasValue)
                {
                    var existingBooking = await _context.MemberBookings.FindAsync(request.BookingId.Value);
                    if (existingBooking != null)
                    {
                        existingBooking.TotalAmount = totalAmount;
                        await _context.SaveChangesAsync();
                    }
                }

                // 4. Create Stripe Checkout Session
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(totalAmount * 100), // Stripe expects cents/paisa
                                Currency = "pkr", // Changed from usd to pkr for Rs. support
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = planName,
                                    Description = $"SpaceHive 360 {request.PlanType} - {planName}",
                                },
                            },
                            Quantity = 1,
                        },
                    },
                    Mode = "payment",
                    SuccessUrl = request.SuccessUrl + "?session_id={CHECKOUT_SESSION_ID}&booking_id=" + (request.BookingId?.ToString() ?? ""),
                    CancelUrl = request.CancelUrl,
                    Metadata = new Dictionary<string, string>
                    {
                        { "booking_id", request.BookingId?.ToString() ?? "" },
                        { "member_user_id", request.MemberUserId.ToString() },
                        { "membership_id", request.MembershipId?.ToString() ?? "" },
                        { "payment_id", request.PaymentId?.ToString() ?? "" }
                    }
                };

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                // 5. Update booking/payment with Session ID
                if (request.BookingId.HasValue)
                {
                    var b = await _context.MemberBookings.FindAsync(request.BookingId.Value);
                    if (b != null)
                    {
                        b.PaymentId = session.Id;
                        await _context.SaveChangesAsync();
                    }
                }

                return ApiResponse.SuccessResponse(new CheckoutSessionResponse
                {
                    SessionId = session.Id,
                    PublicKey = _configuration["Stripe:PublishableKey"] ?? "",
                    CheckoutUrl = session.Url
                });
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Stripe error: " + ex.Message);
            }
        }

        public async Task<ApiResponse> HandleWebhookAsync(string json, string stripeSignature)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _webhookSecret);

                if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;
                    if (session != null)
                    {
                        await UpdateStatusAfterPayment(session);
                    }
                }

                return ApiResponse.SuccessResponse("Webhook handled");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Webhook error: " + ex.Message);
            }
        }

        public async Task<ApiResponse> VerifyPaymentAsync(string sessionId)
        {
            try
            {
                var service = new SessionService();
                var session = await service.GetAsync(sessionId);

                if (session.PaymentStatus == "paid")
                {
                    await UpdateStatusAfterPayment(session);
                    return ApiResponse.SuccessResponse(null, "Payment verified and updated successfully.");
                }

                return ApiResponse.ErrorResponse("Payment not completed yet.", 400);
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Verification error: " + ex.Message);
            }
        }

        private async Task UpdateStatusAfterPayment(Session session)
        {
            var bookingIdStr = session.Metadata.ContainsKey("booking_id") ? session.Metadata["booking_id"] : null;
            if (!string.IsNullOrEmpty(bookingIdStr) && Guid.TryParse(bookingIdStr, out Guid bookingId))
            {
                var booking = await _context.MemberBookings.FindAsync(bookingId);
                if (booking != null)
                {
                    booking.PaymentStatus = "Completed";
                    booking.BookingStatus = "Confirmed";
                    booking.PaymentId = session.PaymentIntentId; // Store actual payment intent
                    await _context.SaveChangesAsync();
                }
            }

            var paymentIdStr = session.Metadata.ContainsKey("payment_id") ? session.Metadata["payment_id"] : null;
            if (!string.IsNullOrEmpty(paymentIdStr) && Guid.TryParse(paymentIdStr, out Guid paymentId))
            {
                var payment = await _context.MemberPayments.FindAsync(paymentId);
                if (payment != null)
                {
                    payment.PaymentStatus = "Completed";
                    payment.ApprovalStatus = "Approved";
                    payment.TransactionId = session.PaymentIntentId;
                    payment.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    // Update all linked memberships (Decoupled update)
                    var memberships = await _context.MemberMemberships
                        .Where(m => m.FkPayment == paymentId)
                        .ToListAsync();

                    foreach (var m in memberships)
                    {
                        m.MembershipStatus = "Active";
                    }

                    // Update all linked bookings (if any link back to this payment)
                    var bookings = await _context.MemberBookings
                        .Where(b => b.FkPayment == paymentId)
                        .ToListAsync();

                    foreach (var b in bookings)
                    {
                        b.PaymentStatus = "Completed";
                        b.BookingStatus = "Confirmed";
                        b.PaymentId = session.PaymentIntentId;
                    }

                    await _context.SaveChangesAsync();

                    // Trigger Availability Update for all affected workspaces
                    var affectedWorkspaceIds = await (from m in _context.MemberMemberships
                                                   join p in _context.PlanMemberships on m.FkPlan equals p.RecId
                                                   where m.FkPayment == paymentId
                                                   select p.FkWorkspace).ToListAsync();

                    foreach (var workspaceId in affectedWorkspaceIds)
                    {
                        if (workspaceId.HasValue)
                        {
                            await _inventoryService.UpdateWorkspaceAvailabilityAsync(workspaceId.Value);
                        }
                    }
                }
            }
        }
    }
}
