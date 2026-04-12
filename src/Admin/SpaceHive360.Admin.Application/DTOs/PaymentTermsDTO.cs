using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SpaceHive360.Admin.Application.DTOs
{
    public class PaymentTermsDTO
    {
        public Guid RecId { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public int BillingCycleMonths { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class PaymentTermsCreateDTO
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public int BillingCycleMonths { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class PaymentTermsUpdateDTO
    {
        [Required]
        public Guid RecId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public int BillingCycleMonths { get; set; }

        public bool IsActive { get; set; }
    }
}
