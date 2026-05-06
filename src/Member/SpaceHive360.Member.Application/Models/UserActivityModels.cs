using System;

namespace SpaceHive360.Member.Application.Models
{
    public class UserActivityRequest
    {
        public Guid? MemberId { get; set; }
        public string WorkspaceId { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
    }

    public class UserActivityBatchRequest
    {
        public System.Collections.Generic.List<UserActivityRequest> Activities { get; set; } = new();
    }
}
