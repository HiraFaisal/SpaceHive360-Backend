using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Application.DTOs
{
    public class FeedbackDTO
    {
        public Guid RecId { get; set; }

        public Guid? FkLocation { get; set; }
        public string? LocationName { get; set; }

        public Guid? FkPayment { get; set; }

        public string? MemberName { get; set; }
        public string? MemberEmail { get; set; }

        public string? Category { get; set; }
        public string? Experience { get; set; }

        public string Comments { get; set; } = null!;

        public int? Rating { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? Sentiment { get; set; }
        public double? SentimentScore { get; set; }
    }
}
