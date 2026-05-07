using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpaceHive360.Member.Application.Models;
using SpaceHive360.Member.Application.Services.Community;
using SpaceHive360.Member.Domain.Entities.Community;
using SpaceHive360.Member.Infrastructure.Data;

namespace SpaceHive360.Member.Infrastructure.Services.Community
{
    public class CommunityService : ICommunityService
    {
        private readonly MemberDbContext _context;

        public CommunityService(MemberDbContext context)
        {
            _context = context;
        }

        public async Task<List<CommunityPostDto>> GetPostsAsync(Guid memberId, string? tag = null)
        {
            var query = _context.CommunityPosts.AsQueryable();

            if (!string.IsNullOrEmpty(tag) && tag != "All Posts")
            {
                query = query.Where(p => p.Tag == tag.ToUpper());
            }

            var posts = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var likedPostIds = await _context.CommunityLikes
                .Where(l => l.MemberId == memberId)
                .Select(l => l.FkPost)
                .ToListAsync();

            return posts.Select(p => new CommunityPostDto
            {
                RecId = p.RecId,
                AuthorName = p.AuthorName,
                AuthorRole = p.AuthorRole,
                CompanyName = p.FkCompany != null ? "Official Update" : "Community Member", // Simplified for now
                AuthorLocation = p.AuthorLocation,
                AuthorInitials = p.AuthorInitials,
                Content = p.Content,
                Tag = p.Tag,
                TagColor = p.TagColor,
                HasImage = p.HasImage,
                ImageUrl = p.ImageUrl,
                LikesCount = p.LikesCount,
                CommentsCount = p.CommentsCount,
                CreatedAt = p.CreatedAt,
                IsLikedByMe = likedPostIds.Contains(p.RecId)
            }).ToList();
        }

        public async Task<CommunityPostDto> CreatePostAsync(CreatePostRequest request, Guid memberId)
        {
            var member = await _context.MemberUsers.FindAsync(memberId);
            if (member == null) throw new Exception("Member not found");

            var post = new CommunityPost
            {
                AuthorName = member.FullName,
                AuthorRole = "Member",
                AuthorLocation = "Hub", // Could be refined if we had member location
                AuthorInitials = member.FullName.Substring(0, 1).ToUpper(),
                Content = request.Content,
                Tag = request.Tag.ToUpper(),
                HasImage = !string.IsNullOrEmpty(request.ImageUrl),
                ImageUrl = request.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

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

        public async Task<CommunityCommentDto> AddCommentAsync(CreateCommentRequest request, Guid memberId)
        {
            var member = await _context.MemberUsers.FindAsync(memberId);
            if (member == null) throw new Exception("Member not found");

            var comment = new CommunityComment
            {
                FkPost = request.PostId,
                MemberName = member.FullName,
                MemberInitials = member.FullName.Substring(0, 1).ToUpper(),
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

        public async Task<List<CommunityEventDto>> GetUpcomingEventsAsync()
        {
            var now = DateTime.UtcNow;
            var events = await _context.CommunityEvents
                .Where(e => e.EventDate >= now.Date)
                .OrderBy(e => e.EventDate)
                .Take(5)
                .ToListAsync();

            return events.Select(e => new CommunityEventDto
            {
                RecId = e.RecId,
                Title = e.Title,
                Description = e.Description,
                Month = e.EventDate.ToString("MMM"),
                Day = e.EventDate.Day.ToString(),
                Time = e.EventDate.ToString("hh:mm tt"),
                Location = e.Location,
                AttendeeInitials = new List<string> { "JD", "AS", "MK" }, // Mock for now
                ExtraCount = 12
            }).ToList();
        }
    }
}
