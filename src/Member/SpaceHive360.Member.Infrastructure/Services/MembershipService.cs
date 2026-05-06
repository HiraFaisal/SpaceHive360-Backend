using Microsoft.EntityFrameworkCore;
using SpaceHive360.Member.Application.Models;
using SpaceHive360.Member.Application.Services;
using SpaceHive360.Member.Domain.Entities;
using SpaceHive360.Member.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Infrastructure.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly MemberDbContext _context;
        private readonly IPaymentService _paymentService;
        private readonly IFileService _fileService;
        private readonly IInventoryService _inventoryService;

        public MembershipService(MemberDbContext context, IPaymentService paymentService, IFileService fileService, IInventoryService inventoryService)
        {
            _context = context;
            _paymentService = paymentService;
            _fileService = fileService;
            _inventoryService = inventoryService;
        }

        public async Task<ApiResponse> PurchaseMembershipAsync(MembershipPurchaseRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Fetch ONLY Membership Plan
                var plan = await _context.PlanMemberships.FirstOrDefaultAsync(p => p.RecId == request.PlanId);
                if (plan == null) return ApiResponse.ErrorResponse("Membership plan not found");

                // Concurrency Lock: Lock the workspace associated with this plan to prevent race conditions
                var workspace = await _context.Workspaces.FromSqlRaw("SELECT * FROM tbl_workspaces WHERE rec_id = {0} FOR UPDATE", plan.FkWorkspace).FirstOrDefaultAsync();
                if (workspace == null) return ApiResponse.ErrorResponse("Associated workspace not found");

                var member = await _context.MemberUsers.FirstOrDefaultAsync(m => m.RecId == request.MemberUserId);
                if (member == null) return ApiResponse.ErrorResponse("Member not found");

                // 2. Determine Limit and Check Slots
                int limit = workspace.InventoryType == "UNIT" ? 1 : (workspace.MaxUnits > 0 ? workspace.MaxUnits : (workspace.Capacity ?? 1));
                
                // Count Active or Pending memberships for this workspace
                var activeCount = await (from m in _context.MemberMemberships
                                       join p in _context.PlanMemberships on m.FkPlan equals p.RecId
                                       where p.FkWorkspace == workspace.RecId 
                                       && (m.MembershipStatus == "Active" || m.MembershipStatus == "Pending")
                                       && m.EndDate > DateTime.UtcNow
                                       select m).CountAsync();

                if (activeCount >= limit)
                {
                    return ApiResponse.ErrorResponse($"No slots available. (Current: {activeCount}, Limit: {limit})");
                }

                // 3. Calculate Dates
                DateTime startDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc);
                DateTime endDate = startDate;
                
                string dType = plan.DurationType?.ToLower() ?? "month";
                int dValue = plan.DurationValue ?? 1;

                if (dType.Contains("month")) endDate = startDate.AddMonths(dValue);
                else if (dType.Contains("day")) endDate = startDate.AddDays(dValue);
                else if (dType.Contains("year")) endDate = startDate.AddYears(dValue);
                else endDate = startDate.AddMonths(dValue); // Default to month if unknown

                // 5. Create Payment Record
                var payment = new MemberPayment
                {
                    FkMemberUser = request.MemberUserId,
                    TotalAmount = plan.Price ?? 0,
                    PaymentMethod = request.PaymentMethod,
                    PaymentStatus = "Pending",
                    ApprovalStatus = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                if (request.PaymentMethod == "BankTransfer")
                {
                    payment.AccountTitle = request.AccountTitle;
                    payment.AccountNumber = request.AccountNumber;
                    payment.IbanNumber = request.IbanNumber;
                    payment.BankName = request.BankName;

                    if (request.PaymentScreenshot != null)
                    {
                        payment.PaymentProof = await _fileService.SaveFileAsync(request.PaymentScreenshot, "payments");
                    }
                    else
                    {
                        return ApiResponse.ErrorResponse("Payment screenshot is required for bank transfer");
                    }
                }

                _context.MemberPayments.Add(payment);
                await _context.SaveChangesAsync();

                // 6. Create Membership Record
                var membership = new MemberMembership
                {
                    FkMemberUser = request.MemberUserId,
                    FkPlan = request.PlanId,
                    StartDate = startDate,
                    EndDate = endDate,
                    Amount = plan.Price ?? 0,
                    MembershipStatus = "Pending",
                    FkPayment = payment.RecId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.MemberMemberships.Add(membership);
                await _context.SaveChangesAsync();

                // 7. Handle Stripe specific logic
                ApiResponse? stripeResponse = null;
                if (request.PaymentMethod == "Stripe")
                {
                    var stripeRequest = new CreateCheckoutSessionRequest
                    {
                        PlanId = request.PlanId,
                        PlanType = "Membership",
                        MemberUserId = request.MemberUserId,
                        MembershipId = membership.RecId,
                        PaymentId = payment.RecId,
                        Amount = plan.Price ?? 0,
                        SuccessUrl = request.SuccessUrl ?? "",
                        CancelUrl = request.CancelUrl ?? ""
                    };

                    stripeResponse = await _paymentService.CreateCheckoutSessionAsync(stripeRequest);
                    if (!stripeResponse.Success)
                    {
                        await transaction.RollbackAsync();
                        return stripeResponse;
                    }

                    if (stripeResponse.Data is CheckoutSessionResponse sessionData)
                    {
                        payment.TransactionId = sessionData.SessionId;
                        await _context.SaveChangesAsync();
                    }
                }

                // 8. Auto Availability Handling
                await _inventoryService.UpdateWorkspaceAvailabilityAsync(workspace.RecId);

                await transaction.CommitAsync();

                if (request.PaymentMethod == "Stripe" && stripeResponse != null) 
                {
                    return ApiResponse.SuccessResponse(stripeResponse.Data);
                }
                
                return ApiResponse.SuccessResponse(new { membershipId = membership.RecId, paymentId = payment.RecId }, "Membership request submitted successfully and is pending approval.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse.ErrorResponse("Failed to purchase membership", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse> PurchaseBookingAsync(BookingPurchaseRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Fetch Plan
                var plan = await _context.PlanBookings.FirstOrDefaultAsync(p => p.RecId == request.PlanId);
                if (plan == null) return ApiResponse.ErrorResponse("Booking plan not found");

                // Concurrency Lock
                var workspace = await _context.Workspaces.FromSqlRaw("SELECT * FROM tbl_workspaces WHERE rec_id = {0} FOR UPDATE", plan.FkWorkspace).FirstOrDefaultAsync();
                if (workspace == null) return ApiResponse.ErrorResponse("Associated workspace not found");

                // 2. Fetch Member
                var member = await _context.MemberUsers.FirstOrDefaultAsync(m => m.RecId == request.MemberUserId);
                if (member == null) return ApiResponse.ErrorResponse("Member not found");

                // 2.5 Inventory Validation (UNIT vs SEAT)
                DateTime reqStart = DateTime.SpecifyKind(request.StartTime, DateTimeKind.Utc);
                DateTime reqEnd = DateTime.SpecifyKind(request.EndTime, DateTimeKind.Utc);

                if (workspace.InventoryType == "UNIT")
                {
                    var conflictExists = await (from b in _context.MemberBookings
                                              join d in _context.BookingDetails on b.RecId equals d.FkBooking
                                              where b.BookingStatus != "Cancelled" && b.BookingStatus != "Rejected"
                                              && d.StartTime < reqEnd && d.EndTime > reqStart
                                              && plan.FkWorkspace == workspace.RecId
                                              select b).AnyAsync();

                    if (conflictExists)
                    {
                        return ApiResponse.ErrorResponse("This workspace is already booked for the selected time slot.");
                    }
                }
                else if (workspace.InventoryType == "SEAT")
                {
                    int limit = workspace.MaxUnits > 0 ? workspace.MaxUnits : (workspace.Capacity ?? 1);
                    var totalSeatsUsed = await (from b in _context.MemberBookings
                                              join d in _context.BookingDetails on b.RecId equals d.FkBooking
                                              where b.BookingStatus != "Cancelled" && b.BookingStatus != "Rejected"
                                              && d.StartTime < reqEnd && d.EndTime > reqStart
                                              && plan.FkWorkspace == workspace.RecId
                                              select b).CountAsync();

                    if (totalSeatsUsed >= limit)
                    {
                        return ApiResponse.ErrorResponse($"No more seats available. (Current: {totalSeatsUsed}, Limit: {limit})");
                    }
                }

                // 4. Create Payment Record
                var payment = new MemberPayment
                {
                    FkMemberUser = request.MemberUserId,
                    TotalAmount = plan.Price ?? 0,
                    PaymentMethod = request.PaymentMethod,
                    PaymentStatus = "Pending",
                    ApprovalStatus = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                if (request.PaymentMethod == "BankTransfer")
                {
                    payment.AccountTitle = request.AccountTitle;
                    payment.AccountNumber = request.AccountNumber;
                    payment.IbanNumber = request.IbanNumber;
                    payment.BankName = request.BankName;

                    if (request.PaymentScreenshot != null)
                    {
                        payment.PaymentProof = await _fileService.SaveFileAsync(request.PaymentScreenshot, "payments");
                    }
                    else
                    {
                        return ApiResponse.ErrorResponse("Payment screenshot is required for bank transfer");
                    }
                }

                _context.MemberPayments.Add(payment);
                await _context.SaveChangesAsync();

                // 5. Calculate Total Price
                decimal basePrice = plan.Price ?? 0;
                decimal durationValue = 1;
                
                if (plan.PricingType?.ToLower() == "per_hour" || plan.PricingType?.ToLower() == "hourly")
                {
                    var timeDiff = (request.EndTime - request.StartTime).TotalHours;
                    durationValue = (decimal)Math.Max(1, timeDiff);
                }

                int occurrencesCount = 1;
                if (request.IsRecurring)
                {
                    if (request.EndType == "After")
                    {
                        occurrencesCount = request.EndAfterOccurrences ?? 1;
                    }
                    else if (request.EndType == "On" && request.RecurrenceEndDate.HasValue)
                    {
                        var start = request.StartTime;
                        var end = request.RecurrenceEndDate.Value;
                        var targetDay = start.DayOfWeek;
                        
                        int count = 0;
                        var current = start.Date;
                        while (current <= end.Date)
                        {
                            if (current.DayOfWeek == targetDay) count++;
                            current = current.AddDays(1);
                            if (count > 100) break;
                        }
                        occurrencesCount = Math.Max(1, count);
                    }
                }

                decimal calculatedSubtotal = (basePrice * durationValue) * (decimal)occurrencesCount;
                decimal calculatedTax = calculatedSubtotal * 0.1m;
                decimal totalToCharge = calculatedSubtotal + calculatedTax;

                var booking = new MemberBooking
                {
                    FkMemberUser = request.MemberUserId,
                    FkPlan = request.PlanId,
                    PlanType = "Booking",
                    BookingDate = DateTime.UtcNow,
                    StartDate = DateTime.SpecifyKind(request.StartTime, DateTimeKind.Utc),
                    EndDate = DateTime.SpecifyKind(request.EndTime, DateTimeKind.Utc),
                    TotalAmount = totalToCharge,
                    PaymentStatus = "Pending",
                    BookingStatus = "Pending",
                    FkPayment = payment.RecId
                };

                _context.MemberBookings.Add(booking);
                await _context.SaveChangesAsync();

                // 6. Create Detailed Booking/Recurrence Record
                var bookingDetail = new MemberBookingDetail
                {
                    FkBooking = booking.RecId,
                    StartTime = DateTime.SpecifyKind(request.StartTime, DateTimeKind.Utc),
                    EndTime = DateTime.SpecifyKind(request.EndTime, DateTimeKind.Utc),
                    IsFullDay = request.IsFullDay,
                    IsRecurring = request.IsRecurring,
                    RecurrenceInterval = request.RecurrenceInterval,
                    RecurrenceType = request.IsRecurring ? "Day" : null,
                    EndType = request.EndType,
                    EndAfterOccurrences = request.EndAfterOccurrences,
                    RecurrenceEndDate = request.RecurrenceEndDate.HasValue 
                        ? DateTime.SpecifyKind(request.RecurrenceEndDate.Value, DateTimeKind.Utc) 
                        : null,
                    SelectedDays = request.SelectedDays,
                    CreatedAt = DateTime.UtcNow
                };

                _context.BookingDetails.Add(bookingDetail);
                await _context.SaveChangesAsync();

                // 7. Handle Stripe specific logic
                ApiResponse? stripeResponse = null;
                if (request.PaymentMethod == "Stripe")
                {
                    var stripeRequest = new CreateCheckoutSessionRequest
                    {
                        PlanId = request.PlanId,
                        PlanType = "Booking",
                        MemberUserId = request.MemberUserId,
                        BookingId = booking.RecId,
                        PaymentId = payment.RecId,
                        Amount = totalToCharge,
                        SuccessUrl = request.SuccessUrl ?? "",
                        CancelUrl = request.CancelUrl ?? ""
                    };

                    stripeResponse = await _paymentService.CreateCheckoutSessionAsync(stripeRequest);
                    if (!stripeResponse.Success)
                    {
                        await transaction.RollbackAsync();
                        return stripeResponse;
                    }

                    if (stripeResponse.Data is CheckoutSessionResponse sessionData)
                    {
                        payment.TransactionId = sessionData.SessionId;
                        await _context.SaveChangesAsync();
                    }
                }

                // 8. Auto Availability Handling for Seat type (Simple check)
                if (workspace.InventoryType == "SEAT")
                {
                    await _inventoryService.UpdateWorkspaceAvailabilityAsync(workspace.RecId);
                }

                await transaction.CommitAsync();

                if (request.PaymentMethod == "Stripe" && stripeResponse != null)
                {
                    return ApiResponse.SuccessResponse(stripeResponse.Data);
                }

                return ApiResponse.SuccessResponse(new { bookingId = booking.RecId, paymentId = payment.RecId }, "Booking request submitted successfully and is pending approval.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse.ErrorResponse("Failed to process booking", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse> GetMyMembershipsAsync(Guid userId)
        {
            try
            {
                var memberships = await (from m in _context.MemberMemberships
                                       join p in _context.PlanMemberships on m.FkPlan equals p.RecId
                                       join w in _context.Workspaces on p.FkWorkspace equals w.RecId
                                       where m.FkMemberUser == userId
                                       select new MyMembershipDto
                                       {
                                           RecId = m.RecId,
                                           WorkspaceName = w.Name ?? "Workspace",
                                           PlanName = p.Name ?? "Plan",
                                           StartDate = m.StartDate,
                                           EndDate = m.EndDate,
                                           Status = m.MembershipStatus,
                                           Amount = m.Amount,
                                           PlanId = m.FkPlan,
                                           Image = p.Images,
                                           RemainingDays = (m.EndDate - DateTime.UtcNow).Days > 0 ? (m.EndDate - DateTime.UtcNow).Days : 0,
                                           IsExtendable = m.MembershipStatus == "Active",
                                           IsRenewable = m.MembershipStatus == "Expired" || (m.MembershipStatus == "Active" && (m.EndDate - DateTime.UtcNow).Days <= 7)
                                       })
                                       .OrderByDescending(m => m.StartDate)
                                       .ToListAsync();

                return ApiResponse.SuccessResponse(memberships);
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to fetch memberships", 500, new List<string> { ex.Message });
            }
        }
    }
}
