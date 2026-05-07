using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Application.DTOs
{
    public class Plans
    {
        public class PlanCreateDto
        {
            public Guid FkCompany { get; set; }
            public Guid FkWorkspaceType { get; set; }
            public Guid FkLocation { get; set; }
            public Guid FkCity { get; set; }
            public Guid? FkPaymentTerm { get; set; }

            public string Name { get; set; } = null!;
            public string? Description { get; set; }

            public int DurationMonths { get; set; }
            public decimal Price { get; set; }

            public bool IsRecurring { get; set; }
            public bool AllowCancellation { get; set; }
            public bool RequiresApproval { get; set; }

            public List<string>? Features { get; set; }
        }

        // =========================
        // UPDATE PLAN DTO
        // =========================
        public class PlanUpdateDto
        {
            public Guid RecId { get; set; }

            public Guid FkWorkspaceType { get; set; }
            public Guid FkLocation { get; set; }
            public Guid FkCity { get; set; }
            public Guid? FkPaymentTerm { get; set; }

            public string Name { get; set; } = null!;
            public string? Description { get; set; }

            public int DurationMonths { get; set; }
            public decimal Price { get; set; }

            public bool IsRecurring { get; set; }
            public bool AllowCancellation { get; set; }
            public bool RequiresApproval { get; set; }

            
            public List<string>? Features { get; set; }

            public bool IsActive { get; set; }
        }

        // =========================
        // PLAN RESPONSE DTO
        // =========================
        public class PlanDto
        {
            public Guid RecId { get; set; }

            public Guid FkCompany { get; set; }
            public Guid FkWorkspaceType { get; set; }
            public Guid FkLocation { get; set; }
            public Guid FkCity { get; set; }
            public Guid? FkPaymentTerm { get; set; }

            public string Name { get; set; } = null!;
            public string? Description { get; set; }

            public int DurationMonths { get; set; }
            public decimal Price { get; set; }

            public bool IsRecurring { get; set; }
            public bool AllowCancellation { get; set; }
            public bool RequiresApproval { get; set; }

            public List<string>? Images { get; set; }
            public List<string>? Features { get; set; }

            public bool IsActive { get; set; }

            public DateTime CreatedAt { get; set; }
        }

        // =========================
        // PAGED LIST DTO (FOR GRID)
        // =========================
        public class PlanListDto
        {
            public Guid RecId { get; set; }

            public string Name { get; set; } = null!;
            public decimal Price { get; set; }

            public int DurationMonths { get; set; }

            public bool IsRecurring { get; set; }
            public bool IsActive { get; set; }

            public string? CityName { get; set; }
            public string? LocationName { get; set; }

            public List<string>? Images { get; set; }
        }
        public class PlanResponseDto
        {
            public Guid RecId { get; set; }
            public Guid? FkCompany { get; set; }
            public Guid? FkWorkspaceType { get; set; }
            public Guid? FkLocation { get; set; }
            public Guid? FkCity { get; set; }
            public Guid? FkPaymentTerm { get; set; }

            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }

            public int DurationMonths { get; set; }
            public decimal Price { get; set; }

            public bool IsRecurring { get; set; }
            public bool AllowCancellation { get; set; }
            public bool RequiresApproval { get; set; }
            public bool IsActive { get; set; }

            public List<string> Images { get; set; } = new();    // ← clean list
            public List<string> Features { get; set; } = new();  // ← clean list

            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdatePriceDto
        {
            public Guid PlanId { get; set; }
            public decimal NewPrice { get; set; }
        }
    }
}
