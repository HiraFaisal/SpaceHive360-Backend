using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Admin.Application.Models;
using SpaceHive360.Admin.Application.Services.Community;

namespace SpaceHive360.Admin.Api.Controllers
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

        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts([FromQuery] string? tag)
        {
            var posts = await _communityService.GetPostsAsync(tag);
            return Ok(posts);
        }

        [HttpPost("posts")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request)
        {
            var post = await _communityService.CreatePostAsync(request, Guid.Empty);
            return Ok(post);
        }

        [HttpPost("posts/{id}/like")]
        public async Task<IActionResult> LikePost(Guid id)
        {
            // For admin portal, we'll use a mock member ID or the admin ID
            var result = await _communityService.LikePostAsync(id, Guid.Empty);
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
            var comment = await _communityService.AddCommentAsync(request, "SpaceHive Admin", "SA");
            return Ok(comment);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _communityService.GetCommunityStatsAsync();
            return Ok(stats);
        }

        [HttpGet("active-members")]
        public async Task<IActionResult> GetActiveMembers()
        {
            var members = await _communityService.GetActiveMembersAsync();
            return Ok(members);
        }

        [HttpGet("events")]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _communityService.GetUpcomingEventsAsync();
            return Ok(events);
        }

        [HttpPost("events")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
        {
            var communityEvent = await _communityService.CreateEventAsync(request, Guid.Empty);
            return Ok(communityEvent);
        }
    }
}
