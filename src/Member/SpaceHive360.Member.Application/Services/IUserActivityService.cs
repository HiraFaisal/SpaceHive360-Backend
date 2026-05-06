using System.Collections.Generic;
using System.Threading.Tasks;
using SpaceHive360.Member.Application.Models;

namespace SpaceHive360.Member.Application.Services
{
    public interface IUserActivityService
    {
        Task LogActivityAsync(UserActivityRequest request);
        Task LogActivitiesBatchAsync(List<UserActivityRequest> requests);
    }
}
