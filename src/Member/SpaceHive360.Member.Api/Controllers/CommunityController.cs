using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Member.Application.Models;
using SpaceHive360.Member.Application.Services.Community;

namespace SpaceHive360.Member.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommunityController : ControllerBase
    {
        private readonly ICommunityService _communityService;

        public CommunityController(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        private Guid GetMemberId()
        {
            // 1. Try to get from JWT claims
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdStr, out var guid)) return guid;
            
            // 2. Try to get from Query Parameters (aliases: memberId, userId)
            if (Request.Query.TryGetValue("memberId", out var qId) && Guid.TryParse(qId, out var qGuid))
                return qGuid;
            
            if (Request.Query.TryGetValue("userId", out var uId) && Guid.TryParse(uId, out var uGuid))
                return uGuid;

            // 3. Try to get from Headers
            if (Request.Headers.TryGetValue("X-Member-Id", out var hId) && Guid.TryParse(hId, out var hGuid))
                return hGuid;

            return Guid.Empty;
        }

        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts([FromQuery] string? tag)
        {
            var memberId = GetMemberId();
            var posts = await _communityService.GetPostsAsync(memberId, tag);
            return Ok(posts);
        }

        [HttpPost("posts/{id}/like")]
        public async Task<IActionResult> LikePost(Guid id)
        {
            var memberId = GetMemberId();
            if (memberId == Guid.Empty) return Unauthorized();

            var result = await _communityService.LikePostAsync(id, memberId);
            return Ok(new { liked = result });
        }

        [HttpGet("posts/{id}/comments")]
        public async Task<IActionResult> GetComments(Guid id)
        {
            var comments = await _communityService.GetCommentsAsync(id);
            return Ok(comments);
        }

        [HttpPost("comments")]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentRequest request)
        {
            var memberId = GetMemberId();
            if (memberId == Guid.Empty) return Unauthorized();

            var comment = await _communityService.AddCommentAsync(request, memberId);
            return Ok(comment);
        }

        [HttpGet("events")]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _communityService.GetUpcomingEventsAsync();
            return Ok(events);
        }
    }
}
