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

        public PaymentService(IConfiguration configuration, MemberDbContext context)
        {
            _configuration = configuration;
            _context = context;
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

                // 2. Create Pending Booking in DB
                var booking = new MemberBooking
                {
                    RecId = Guid.NewGuid(),
                    FkMemberUser = request.MemberUserId,
                    FkPlan = request.PlanId,
                    PlanType = request.PlanType,
                    TotalAmount = price,
                    PaymentStatus = "Pending",
                    BookingStatus = "Pending",
                    BookingDate = DateTime.UtcNow
                };

                _context.MemberBookings.Add(booking);
                await _context.SaveChangesAsync();

                // 3. Create Stripe Checkout Session
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(price * 100), // Stripe expects cents
                                Currency = "usd",
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
                    SuccessUrl = request.SuccessUrl + "?session_id={CHECKOUT_SESSION_ID}&booking_id=" + booking.RecId,
                    CancelUrl = request.CancelUrl,
                    Metadata = new Dictionary<string, string>
                    {
                        { "booking_id", booking.RecId.ToString() },
                        { "member_user_id", request.MemberUserId.ToString() },
                        { "membership_id", request.MembershipId?.ToString() ?? "" },
                        { "payment_id", request.PaymentId?.ToString() ?? "" }
                    }
                };

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                // Update booking with Payment Session ID
                booking.PaymentId = session.Id;
                await _context.SaveChangesAsync();

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
                        var bookingIdStr = session.Metadata["booking_id"];

                        if (Guid.TryParse(bookingIdStr, out Guid bookingId))
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

                        var paymentIdStr = session.Metadata["payment_id"];
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
                            }
                        }
                    }
                }

                return ApiResponse.SuccessResponse("Webhook handled");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Webhook error: " + ex.Message);
            }
        }
    }
}
