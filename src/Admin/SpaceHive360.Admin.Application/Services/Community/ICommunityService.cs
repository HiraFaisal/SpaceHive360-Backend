using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpaceHive360.Admin.Application.Models;

namespace SpaceHive360.Admin.Application.Services.Community
{
    public interface ICommunityService
    {
        Task<List<CommunityPostDto>> GetPostsAsync(string? tag = null);
        Task<CommunityPostDto> CreatePostAsync(CreatePostRequest request, Guid adminId);
        Task<bool> LikePostAsync(Guid postId, Guid memberId);
        Task<CommunityCommentDto> AddCommentAsync(CreateCommentRequest request, string memberName, string initials);
        Task<List<CommunityCommentDto>> GetCommentsAsync(Guid postId);
        Task<CommunityStatsDto> GetCommunityStatsAsync();
        Task<List<ActiveMemberDto>> GetActiveMembersAsync();

        // Events
        Task<List<CommunityEventDto>> GetUpcomingEventsAsync();
        Task<CommunityEventDto> CreateEventAsync(CreateEventRequest request, Guid adminId);
    }
}
