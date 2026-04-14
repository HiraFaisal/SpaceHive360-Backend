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

        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<List<FeedbackDTO>> GetFeedbacksAsync(Guid? recId, Guid userRecId)
        {
            try
            {
                // 🔥 Just call repository
                var result = await _feedbackRepository.GetFeedbacksAsync(recId, userRecId);

                // (Optional business logic can go here)
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Service error while fetching feedbacks: {ex.Message}", ex);
            }
        }
    }
}
