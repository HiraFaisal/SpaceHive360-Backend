using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpaceHive360.Member.Application.Models;

namespace SpaceHive360.Member.Application.Services.Community
{
    public interface ICommunityService
    {
        Task<List<CommunityPostDto>> GetPostsAsync(Guid memberId, string? tag = null);
        Task<CommunityPostDto> CreatePostAsync(CreatePostRequest request, Guid memberId);
        Task<bool> LikePostAsync(Guid postId, Guid memberId);
        Task<CommunityCommentDto> AddCommentAsync(CreateCommentRequest request, Guid memberId);
        Task<List<CommunityCommentDto>> GetCommentsAsync(Guid postId);
        Task<List<CommunityEventDto>> GetUpcomingEventsAsync();
    }
}
