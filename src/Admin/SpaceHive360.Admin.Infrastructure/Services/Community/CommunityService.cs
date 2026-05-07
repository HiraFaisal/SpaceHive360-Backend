using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpaceHive360.Admin.Application.Models;
using SpaceHive360.Admin.Application.Services.Community;
using SpaceHive360.Admin.Domain.Entities.Community;
using SpaceHive360.Admin.Infrastructure.Data;
using SpaceHive360.Admin.Application.Services.Files;

namespace SpaceHive360.Admin.Infrastructure.Services.Community
{
    public class CommunityService : ICommunityService
    {
        private readonly AdminDbContext _context;
        private readonly IFileService _fileService;

        public CommunityService(AdminDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<List<CommunityPostDto>> GetPostsAsync(string? tag = null)
        {
            var query = _context.CommunityPosts.AsQueryable();

            if (!string.IsNullOrEmpty(tag) && tag != "All Posts")
            {
                query = query.Where(p => p.Tag == tag.ToUpper());
            }

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new CommunityPostDto
                {
                    RecId = p.RecId,
                    AuthorName = p.AuthorName,
                    AuthorRole = p.AuthorRole,
                    AuthorLocation = p.AuthorLocation,
                    AuthorInitials = p.AuthorInitials,
                    Content = p.Content,
                    Tag = p.Tag,
                    TagColor = p.TagColor,
                    HasImage = p.HasImage,
                    ImageUrl = p.ImageUrl,
                    LikesCount = p.LikesCount,
                    CommentsCount = p.CommentsCount,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<CommunityPostDto> CreatePostAsync(CreatePostRequest request, Guid adminId)
        {
            string? imageUrl = null;
            if (request.Image != null)
            {
                var savedUrls = await _fileService.SaveImagesAsync(new List<Microsoft.AspNetCore.Http.IFormFile> { request.Image }, "community");
                imageUrl = savedUrls.FirstOrDefault();
            }

            var post = new CommunityPost
            {
                AuthorName = "SpaceHive Admin",
                AuthorRole = "Community Manager",
                AuthorLocation = "Main Office",
                AuthorInitials = "SA",
                Content = request.Content,
                Tag = request.Tag.ToUpper(),
                HasImage = !string.IsNullOrEmpty(imageUrl),
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow
            };

            // Set tag colors based on type
            post.TagColor = post.Tag switch
            {
                "ANNOUNCEMENT" => "bg-blue-500/10 text-blue-600 border-blue-500/20",
                "COLLAB REQUEST" => "bg-emerald-500/10 text-emerald-600 border-emerald-500/20",
                "EVENT" => "bg-purple-500/10 text-purple-600 border-purple-500/20",
                _ => "bg-gray-500/10 text-gray-600 border-gray-500/20"
            };

            _context.CommunityPosts.Add(post);
            await _context.SaveChangesAsync();

            return new CommunityPostDto
            {
                RecId = post.RecId,
                AuthorName = post.AuthorName,
                AuthorRole = post.AuthorRole,
                AuthorLocation = post.AuthorLocation,
                AuthorInitials = post.AuthorInitials,
                Content = post.Content,
                Tag = post.Tag,
                TagColor = post.TagColor,
                HasImage = post.HasImage,
                ImageUrl = post.ImageUrl,
                LikesCount = post.LikesCount,
                CommentsCount = post.CommentsCount,
                CreatedAt = post.CreatedAt
            };
        }

        public async Task<bool> LikePostAsync(Guid postId, Guid memberId)
        {
            var existingLike = await _context.CommunityLikes
                .FirstOrDefaultAsync(l => l.FkPost == postId && l.MemberId == memberId);

            if (existingLike != null)
            {
                _context.CommunityLikes.Remove(existingLike);
                var p = await _context.CommunityPosts.FindAsync(postId);
                if (p != null) p.LikesCount = Math.Max(0, p.LikesCount - 1);
            }
            else
            {
                _context.CommunityLikes.Add(new CommunityLike
                {
                    FkPost = postId,
                    MemberId = memberId
                });
                var p = await _context.CommunityPosts.FindAsync(postId);
                if (p != null) p.LikesCount++;
            }

            await _context.SaveChangesAsync();
            return existingLike == null;
        }

        public async Task<CommunityCommentDto> AddCommentAsync(CreateCommentRequest request, string memberName, string initials)
        {
            var comment = new CommunityComment
            {
                FkPost = request.PostId,
                MemberName = memberName,
                MemberInitials = initials,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.CommunityComments.Add(comment);
            
            var post = await _context.CommunityPosts.FindAsync(request.PostId);
            if (post != null) post.CommentsCount++;

            await _context.SaveChangesAsync();

            return new CommunityCommentDto
            {
                RecId = comment.RecId,
                MemberName = comment.MemberName,
                MemberInitials = comment.MemberInitials,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task<List<CommunityCommentDto>> GetCommentsAsync(Guid postId)
        {
            return await _context.CommunityComments
                .Where(c => c.FkPost == postId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommunityCommentDto
                {
                    RecId = c.RecId,
                    MemberName = c.MemberName,
                    MemberInitials = c.MemberInitials,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<CommunityStatsDto> GetCommunityStatsAsync()
        {
            var totalPosts = await _context.CommunityPosts.CountAsync();
            var totalMembers = await _context.MemberUsers.CountAsync();
            
            // Define active as having any activity in the last 7 days
            var activeThreshold = DateTime.UtcNow.AddDays(-7);
            var activeMembers = await _context.UserActivities
                .Where(a => a.CreatedAt >= activeThreshold)
                .Select(a => a.FkMember)
                .Distinct()
                .CountAsync();

            return new CommunityStatsDto
            {
                TotalPosts = totalPosts,
                TotalMembers = totalMembers,
                ActiveMembers = activeMembers,
                MonthlyEngagement = totalPosts * 5 // Mock multiplier for demo
            };
        }

        public async Task<List<ActiveMemberDto>> GetActiveMembersAsync()
        {
            // Get members who were active in the last 24 hours
            var threshold = DateTime.UtcNow.AddHours(-24);
            var activeMemberIds = await _context.UserActivities
                .Where(a => a.CreatedAt >= threshold)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => a.FkMember)
                .Distinct()
                .Take(5)
                .ToListAsync();

            if (!activeMemberIds.Any())
            {
                // Fallback: Get most recent members who joined
                activeMemberIds = await _context.MemberUsers
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(5)
                    .Select(m => m.RecId)
                    .ToListAsync();
            }

            var members = await _context.MemberUsers
                .Where(m => activeMemberIds.Contains(m.RecId))
                .ToListAsync();

            return members.Select(m => new ActiveMemberDto
            {
                Name = m.FullName,
                Initials = !string.IsNullOrEmpty(m.FullName) ? m.FullName.Substring(0, 1).ToUpper() : "M",
                Online = true // For demo purposes
            }).ToList();
        }

        public async Task<List<CommunityEventDto>> GetUpcomingEventsAsync()
        {
            var now = DateTime.UtcNow;
            // Get all upcoming events, or even ones from today
            var events = await _context.CommunityEvents
                .Where(e => e.EventDate >= now.Date) // Changed to .Date to show today's events too
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            return events.Select(e => new CommunityEventDto
            {
                RecId = e.RecId,
                Title = e.Title,
                Description = e.Description,
                EventDate = e.EventDate,
                Location = e.Location,
                Month = e.EventDate.ToString("MMM").ToUpper(),
                Day = e.EventDate.Day.ToString(),
                Time = e.EventDate.ToString("t"),
                AttendeeInitials = new List<string> { "SA", "JD", "AS" }, // Mock for admin view
                ExtraCount = 8
            }).ToList();
        }

        public async Task<CommunityEventDto> CreateEventAsync(CreateEventRequest request, Guid adminId)
        {
            var communityEvent = new CommunityEvent
            {
                Title = request.Title,
                Description = request.Description,
                EventDate = request.EventDate,
                Location = request.Location,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = adminId
            };

            _context.CommunityEvents.Add(communityEvent);
            await _context.SaveChangesAsync();

            return new CommunityEventDto
            {
                RecId = communityEvent.RecId,
                Title = communityEvent.Title,
                Description = communityEvent.Description,
                EventDate = communityEvent.EventDate,
                Location = communityEvent.Location,
                Month = communityEvent.EventDate.ToString("MMM").ToUpper(),
                Day = communityEvent.EventDate.Day.ToString(),
                Time = communityEvent.EventDate.ToString("t")
            };
        }
    }
}
