using SpaceHive360.Admin.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Application.IRepositories
{
    public interface IFeedbackRepository
    {
        Task<List<FeedbackDTO>> GetFeedbacksAsync(Guid? recId, Guid userRecId);

        Task<Guid> GetCompanyIdByUserRecIdAsync(Guid userRecId);
    }
}
