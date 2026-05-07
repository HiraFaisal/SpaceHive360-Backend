using System;
using System.Collections.Generic;

namespace SpaceHive360.SuperAdmin.Application.DTOs
{
    public class FaqCategoryDto
    {
        public Guid RecId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = "HelpCircle";
        public int DisplayOrder { get; set; }
        public List<FaqDto> Faqs { get; set; } = new();
    }

    public class FaqDto
    {
        public Guid RecId { get; set; }
        public Guid FkCategory { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string? Steps { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateFaqCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = "HelpCircle";
        public int DisplayOrder { get; set; }
    }

    public class CreateFaqRequest
    {
        public Guid FkCategory { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string? Steps { get; set; }
    }
}
