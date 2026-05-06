using SpaceHive360.Member.Application.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Application.Services
{
    public interface IFeedbackService
    {
        Task<ApiResponse> SubmitFeedbackAsync(FeedbackRequest request);
        Task<ApiResponse> GetFeedbackForBookingAsync(Guid bookingId);
        Task<ApiResponse> GetFeedbackForMembershipAsync(Guid membershipId);
        Task<ApiResponse> GetRecentFeedbackAsync(int count);
    }
}
