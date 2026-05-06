using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace SpaceHive360.Admin.Application.DTOs
{
    public class PlanMemberships
    {
        public class PlanMembershipCreateDto
        {
            public Guid? FkCompany { get; set; }
            public Guid? FkWorkspaceType { get; set; }
            public Guid? FkWorkspace { get; set; }
            public Guid? FkLocation { get; set; }

            public string Name { get; set; } = null!;
            public string? Description { get; set; }
            public string? PlanCategory { get; set; }

            public string? DurationType { get; set; }
            public int? DurationValue { get; set; }

            public string? FkPaymentTerm { get; set; }
            public decimal? Price { get; set; }

            public bool IsRecurring { get; set; }
            public bool AllowCancellation { get; set; }
            public bool RequiresApproval { get; set; }

            public List<string>? Features { get; set; }
            public IFormFileCollection? Images { get; set; }
        }

        public class PlanMembershipUpdateDto : PlanMembershipCreateDto
        {
            public Guid RecId { get; set; }
        }

        public class PlanMembershipDto
        {
            public Guid RecId { get; set; }

            public Guid? FkCompany { get; set; }
            public Guid? FkWorkspaceType { get; set; }
            public Guid? FkWorkspace { get; set; }
            public Guid? FkLocation { get; set; }

            public string Name { get; set; } = null!;
            public string? Description { get; set; }

            public string? DurationType { get; set; }
            public int? DurationValue { get; set; }

            public string? FkPaymentTerm { get; set; }
            public decimal? Price { get; set; }

            public bool IsRecurring { get; set; }
            public bool AllowCancellation { get; set; }
            public bool RequiresApproval { get; set; }

            public List<string>? Images { get; set; }
            public List<string>? Features { get; set; }

            public string? PlanCategory { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }
        public class PlanMembershipStatsDto
        {
            public int TotalPlans { get; set; }
            public int ActivePlans { get; set; }
            public decimal AveragePrice { get; set; }
            public int NewPlansThisMonth { get; set; }
        }
    }
}
