using System;
using System.ComponentModel.DataAnnotations;

namespace SpaceHive360.Admin.Application.DTOs
{
    public class Workspace
    {
        public class WorkspaceTypeRequest
        {
            [Required]
            [MaxLength(150)]
            public string Name { get; set; } = null!;

            public string? Description { get; set; }

            public string? IconUrl { get; set; }

            public bool IsActive { get; set; } = true;
        }

        public class EditWorkspaceTypeRequest
        {
            [Required]
            public Guid RecId { get; set; }
            public WorkspaceTypeRequest WorkspaceType { get; set; }
        }
        public class WorkspaceRequest
        {
            [Required]
            [MaxLength(150)]
            public string Name { get; set; } = null!;

            public string? Description { get; set; }

            public int? Capacity { get; set; }

            public Guid FkWorkspaceType { get; set; }
            public Guid? FkLocation { get; set; }

            public bool IsActive { get; set; } = true;
            public bool IsAvailable { get; set; } = true;
        }
        public class EditWorkspaceRequest
        {
            [Required]
            public Guid RecId { get; set; }
            public WorkspaceRequest Workspace { get; set; }
        }
    }
}
