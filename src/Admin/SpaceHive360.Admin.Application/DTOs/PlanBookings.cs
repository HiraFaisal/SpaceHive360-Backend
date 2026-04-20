using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace SpaceHive360.Admin.Application.DTOs
{
    public class PlanBookings
    {
        public class PlanBookingCreateDto
        {
            public Guid? FkCompany { get; set; }
            public Guid? FkWorkspace { get; set; }
            public Guid? FkWorkspaceType { get; set; }
            public Guid? FkLocation { get; set; }

            public string Name { get; set; } = null!;
            public string? Description { get; set; }

            public TimeSpan? StartTime { get; set; }
            public TimeSpan? EndTime { get; set; }
            public List<string>? AvailableDays { get; set; }
            
            public int? MinDurationMinutes { get; set; }
            public int? MaxDurationMinutes { get; set; }

            public string? PriceType { get; set; }
            public decimal? Price { get; set; }

            public bool AllowCancellation { get; set; }
            public bool RequiresApproval { get; set; }

            public List<string>? Features { get; set; }
            public IFormFileCollection? Images { get; set; }
        }

        public class PlanBookingUpdateDto : PlanBookingCreateDto
        {
            public Guid RecId { get; set; }
        }

        public class PlanBookingDto
        {
            public Guid RecId { get; set; }

            public Guid? FkCompany { get; set; }
            public Guid? FkWorkspace { get; set; }
            public Guid? FkWorkspaceType { get; set; }
            public Guid? FkLocation { get; set; }

            public string Name { get; set; } = null!;
            public string? Description { get; set; }

            public TimeSpan? StartTime { get; set; }
            public TimeSpan? EndTime { get; set; }
            public List<string>? AvailableDays { get; set; }

            public int? MinDurationMinutes { get; set; }
            public int? MaxDurationMinutes { get; set; }

            public string? PlanCategory { get; set; }
            public string? PriceType { get; set; }
            public decimal? Price { get; set; }

            public bool AllowCancellation { get; set; }
            public bool RequiresApproval { get; set; }

            public List<string>? Images { get; set; }
            public List<string>? Features { get; set; }

            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }
    }
}
