using System;
using System.Collections.Generic;

namespace SpaceHive360.Member.Application.Models
{
    public class FeedbackRequest
    {
        public Guid? PlanBookingId { get; set; }
        public Guid? PlanMembershipId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? UserId { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class FeedbackDTO
    {
        public Guid RecId { get; set; }
        public Guid? PlanBookingId { get; set; }
        public Guid? PlanMembershipId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? WorkspaceName { get; set; }
        public string? PlanName { get; set; }
        public string? Sentiment { get; set; }
        public float? SentimentScore { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class FeedbackSummary
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int RatingCount1 { get; set; }
        public int RatingCount2 { get; set; }
        public int RatingCount3 { get; set; }
        public int RatingCount4 { get; set; }
        public int RatingCount5 { get; set; }
        public List<FeedbackDTO> Feedbacks { get; set; } = new List<FeedbackDTO>();
    }
}
