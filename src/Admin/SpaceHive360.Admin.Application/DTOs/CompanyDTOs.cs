using System;
using System.ComponentModel.DataAnnotations;

namespace SpaceHive360.Admin.Application.DTOs
{
    public class CompanyRegistrationDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [MaxLength(30)]
        public string? Phone { get; set; }

        public string? Address { get; set; }
        public string? Website { get; set; }
        public string? TaxId { get; set; }
        public string? ContactPersonName { get; set; }
    }

    public class CompanyApprovalDto
    {
        public string? AdminComments { get; set; }
    }

    public class CompanyDto
    {
        public Guid RecId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? LogoUrl { get; set; }
        public string? Website { get; set; }
        public string? TaxId { get; set; }
        public string? ContactPersonName { get; set; }
        public string RegistrationStatus { get; set; } = null!;
        public string? AdminComments { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
