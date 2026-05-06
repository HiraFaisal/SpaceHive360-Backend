using System;

namespace SpaceHive360.Admin.Application.DTOs
{
    public class LocationSentimentSummaryDTO
    {
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public double PositivePercentage { get; set; }
        public double NeutralPercentage { get; set; }
        public double NegativePercentage { get; set; }
        public int TotalReviews { get; set; }
        public string AiSummary { get; set; } = string.Empty;
    }
}
