using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SpaceHive360.Admin.Application.DTOs
{
    public class LocationDTO
    {
        public Guid RecId { get; set; }

        public Guid FkCity { get; set; }

        public string Name { get; set; } = null!;

        public string? Address { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public bool IsActive { get; set; }
    }

    public class LocationCreateDTO
    {
        [Required]
        public Guid FkCity { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        public string? Address { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }
    }

    public class LocationUpdateDTO
    {
        [Required]
        public Guid RecId { get; set; }

        [Required]
        public Guid FkCity { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        public string? Address { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public bool IsActive { get; set; }
    }
}
