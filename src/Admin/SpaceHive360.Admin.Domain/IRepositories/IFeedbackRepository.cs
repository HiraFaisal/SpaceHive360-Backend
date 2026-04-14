using SpaceHive360.Admin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Domain.IRepositories
{
    public interface IFeedbackRepository
    {
        //Task<List<Feedback>> GetAllAsync(Guid userRecId);

        //Task<Feedback?> GetByIdAsync(Guid id, Guid userRecId);

        //Task<List<SpaceHive360.Admin.Application.DTOs>> GetFeedbacksAsync(Guid? recId, Guid userRecId);

        Task<Guid> GetCompanyIdByUserRecIdAsync(Guid userRecId);
    }
}
