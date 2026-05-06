using System;

namespace SpaceHive360.SuperAdmin.Application.DTOs
{
    public class SuperAdminCompanyResponseDto
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public Guid? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? Remarks { get; set; }
        public string? ContactPersonName { get; set; }
    }

    public class CompanyActionRequestDto
    {
        public string? Remarks { get; set; }
    }
}
