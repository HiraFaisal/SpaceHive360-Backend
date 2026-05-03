using SpaceHive360.Admin.Application.DTOs;
using SpaceHive360.Admin.Application.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Application.Services.Feedback
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly Services.Ai.IAiService _aiService;

        public FeedbackService(IFeedbackRepository feedbackRepository, Services.Ai.IAiService aiService)
        {
            _feedbackRepository = feedbackRepository;
            _aiService = aiService;
        }

        public async Task<List<FeedbackDTO>> GetFeedbacksAsync(Guid? recId, Guid userRecId)
        {
            try
            {
                var result = await _feedbackRepository.GetFeedbacksAsync(recId, userRecId);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Service error while fetching feedbacks: {ex.Message}", ex);
            }
        }

        public async Task<LocationSentimentSummaryDTO> GetLocationSentimentSummaryAsync(Guid locationId, Guid userRecId)
        {
            try
            {
                var feedbacks = await _feedbackRepository.GetFeedbacksByLocationAsync(locationId);
                
                if (feedbacks == null || feedbacks.Count == 0)
                {
                    return new LocationSentimentSummaryDTO
                    {
                        LocationId = locationId,
                        AiSummary = "No feedback available for this location yet."
                    };
                }

                var total = feedbacks.Count;
                var positive = feedbacks.Count(f => f.Sentiment?.ToLower() == "positive");
                var neutral = feedbacks.Count(f => f.Sentiment?.ToLower() == "neutral");
                var negative = feedbacks.Count(f => f.Sentiment?.ToLower() == "negative");

                // If sentiment is not stored, we could trigger analysis here for a few or wait for a background job
                // For now, we assume it's stored.

                var comments = feedbacks
                    .Where(f => !string.IsNullOrEmpty(f.Comments))
                    .Select(f => f.Comments)
                    .Take(10) // Limit for summarization performance
                    .ToList();

                var aiSummary = await _aiService.SummarizeFeedbackAsync(comments);

                return new LocationSentimentSummaryDTO
                {
                    LocationId = locationId,
                    LocationName = feedbacks.FirstOrDefault()?.LocationName ?? "Unknown",
                    TotalReviews = total,
                    PositivePercentage = total > 0 ? (double)positive / total * 100 : 0,
                    NeutralPercentage = total > 0 ? (double)neutral / total * 100 : 0,
                    NegativePercentage = total > 0 ? (double)negative / total * 100 : 0,
                    AiSummary = aiSummary
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating sentiment summary: {ex.Message}", ex);
            }
        }
    }
}
