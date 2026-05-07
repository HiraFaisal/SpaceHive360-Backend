using System;
using System.Collections.Generic;

namespace SpaceHive360.Member.Application.Models
{
    public class CommunityPostDto
    {
        public Guid RecId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorRole { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string AuthorLocation { get; set; } = string.Empty;
        public string AuthorInitials { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string TagColor { get; set; } = string.Empty;
        public bool HasImage { get; set; }
        public string? ImageUrl { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsLikedByMe { get; set; }
    }

    public class CreatePostRequest
    {
        public string Content { get; set; } = string.Empty;
        public string Tag { get; set; } = "GENERAL";
        public string? ImageUrl { get; set; }
    }

    public class CreateCommentRequest
    {
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class CommunityCommentDto
    {
        public Guid RecId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string MemberInitials { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CommunityStatsDto
    {
        public int TotalPosts { get; set; }
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int MonthlyEngagement { get; set; }
    }

    public class ActiveMemberDto
    {
        public string Name { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty;
        public bool Online { get; set; }
    }

    public class CommunityEventDto
    {
        public Guid RecId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Month { get; set; } = string.Empty;
        public string Day { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public List<string> AttendeeInitials { get; set; } = new();
        public int ExtraCount { get; set; }
    }

    public class CreateEventRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EventDate { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
