using Microsoft.EntityFrameworkCore;
using SpaceHive360.Member.Application.Models;
using SpaceHive360.Member.Application.Services;
using SpaceHive360.Member.Domain.Entities;
using SpaceHive360.Member.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Infrastructure.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly MemberDbContext _context;
        private readonly Application.Services.Ai.IAiService _aiService;

        public FeedbackService(MemberDbContext context, Application.Services.Ai.IAiService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        public async Task<ApiResponse> SubmitFeedbackAsync(FeedbackRequest request)
        {
            try
            {
                if (request.PlanBookingId == null && request.PlanMembershipId == null)
                {
                    return ApiResponse.ErrorResponse("At least one of booking_id or membership_id must exist");
                }

                Guid companyId = request.CompanyId ?? Guid.Empty;
                Guid? locationId = null;

                if (request.PlanBookingId.HasValue)
                {
                    locationId = await _context.PlanBookings
                        .Where(b => b.RecId == request.PlanBookingId.Value)
                        .Select(b => b.FkLocation)
                        .FirstOrDefaultAsync();
                }
                else if (request.PlanMembershipId.HasValue)
                {
                    locationId = await _context.PlanMemberships
                        .Where(m => m.RecId == request.PlanMembershipId.Value)
                        .Select(m => m.FkLocation)
                        .FirstOrDefaultAsync();
                }

                var user = await _context.MemberUsers
                    .FirstOrDefaultAsync(u => u.RecId.ToString() == request.UserId);

                // Sentiment analysis is now handled on the frontend
                var sentiment = request.Sentiment ?? "neutral";
                var score = request.SentimentScore ?? 0.5f;

                var feedback = new Feedback
                {
                    RecId = Guid.NewGuid(),
                    FkCompany = companyId,
                    FkLocation = locationId,
                    PlanBookingId = request.PlanBookingId,
                    PlanMembershipId = request.PlanMembershipId,
                    Rating = request.Rating,
                    Comments = request.Comment ?? "",
                    Sentiment = sentiment,
                    SentimentScore = score,
                    MemberName = user?.FullName,
                    MemberEmail = user?.Email,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                return ApiResponse.SuccessResponse("Feedback submitted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to submit feedback: " + ex.Message);
            }
        }

        public async Task<ApiResponse> GetFeedbackForBookingAsync(Guid bookingId)
        {
            try
            {
                var query = _context.Feedbacks.Where(f => f.PlanBookingId == bookingId && f.IsActive);
                return await GetFeedbackSummaryAsync(query, bookingId, null);
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to get feedback: " + ex.Message);
            }
        }

        public async Task<ApiResponse> GetFeedbackForMembershipAsync(Guid membershipId)
        {
            try
            {
                var query = _context.Feedbacks.Where(f => f.PlanMembershipId == membershipId && f.IsActive);
                return await GetFeedbackSummaryAsync(query, null, membershipId);
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to get feedback: " + ex.Message);
            }
        }

        private async Task<ApiResponse> GetFeedbackSummaryAsync(IQueryable<Feedback> query, Guid? bookingId, Guid? membershipId)
        {
            var feedbacks = await query
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new FeedbackDTO
                {
                    RecId = f.RecId,
                    PlanBookingId = f.PlanBookingId,
                    PlanMembershipId = f.PlanMembershipId,
                    Rating = f.Rating ?? 0,
                    Comment = f.Comments,
                    UserId = f.MemberEmail,
                    UserName = f.MemberName,
                    CreatedAt = f.CreatedAt,
                    PlanName = bookingId != null 
                        ? _context.PlanBookings.Where(pb => pb.RecId == bookingId).Select(pb => pb.Name).FirstOrDefault()
                        : _context.PlanMemberships.Where(pm => pm.RecId == membershipId).Select(pm => pm.Name).FirstOrDefault()
                })
                .ToListAsync();

            var totalReviews = feedbacks.Count;
            var averageRating = totalReviews > 0 ? Math.Round(feedbacks.Average(f => f.Rating), 1) : 0;

            var summary = new FeedbackSummary
            {
                AverageRating = averageRating,
                TotalReviews = totalReviews,
                RatingCount1 = feedbacks.Count(f => f.Rating == 1),
                RatingCount2 = feedbacks.Count(f => f.Rating == 2),
                RatingCount3 = feedbacks.Count(f => f.Rating == 3),
                RatingCount4 = feedbacks.Count(f => f.Rating == 4),
                RatingCount5 = feedbacks.Count(f => f.Rating == 5),
                Feedbacks = feedbacks
            };

            return ApiResponse.SuccessResponse(summary);
        }

        public async Task<ApiResponse> GetRecentFeedbackAsync(int count)
        {
            try
            {
                var feedbacks = await _context.Feedbacks
                    .Where(f => f.IsActive)
                    .OrderByDescending(f => f.CreatedAt)
                    .Take(count)
                    .Select(f => new FeedbackDTO
                    {
                        RecId = f.RecId,
                        PlanBookingId = f.PlanBookingId,
                        PlanMembershipId = f.PlanMembershipId,
                        Rating = f.Rating ?? 0,
                        Comment = f.Comments,
                        UserId = f.MemberEmail,
                        UserName = f.MemberName,
                        CreatedAt = f.CreatedAt,
                        PlanName = f.PlanBookingId != null 
                            ? _context.PlanBookings.Where(pb => pb.RecId == f.PlanBookingId).Select(pb => pb.Name).FirstOrDefault()
                            : _context.PlanMemberships.Where(pm => pm.RecId == f.PlanMembershipId).Select(pm => pm.Name).FirstOrDefault()
                    })
                    .ToListAsync();

                return ApiResponse.SuccessResponse(feedbacks);
            }
            catch (Exception ex)
            {
                return ApiResponse.ErrorResponse("Failed to get recent feedback: " + ex.Message);
            }
        }
    }
}
