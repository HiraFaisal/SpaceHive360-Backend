using SpaceHive360.Admin.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Application.Services.Feedback
{
    public interface IFeedbackService
    {
        Task<List<FeedbackDTO>> GetFeedbacksAsync(Guid? recId, Guid userRecId);
        Task<LocationSentimentSummaryDTO> GetLocationSentimentSummaryAsync(Guid locationId, Guid userRecId);
    }
}
