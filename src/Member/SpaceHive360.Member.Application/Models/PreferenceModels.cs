using System;
using System.Collections.Generic;

namespace SpaceHive360.Member.Application.Models
{
    public class UserPreferenceRequest
    {
        public decimal? BudgetMin { get; set; }
        public decimal? BudgetMax { get; set; }
        public double? PreferredLat { get; set; }
        public double? PreferredLng { get; set; }
        public string? Environment { get; set; }
        public List<string> Amenities { get; set; } = new List<string>();
    }
}
