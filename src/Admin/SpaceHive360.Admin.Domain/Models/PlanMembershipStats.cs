using System;

namespace SpaceHive360.Admin.Domain.Models
{
    public class PlanMembershipStats
    {
        public int TotalPlans { get; set; }
        public int ActivePlans { get; set; }
        public decimal AveragePrice { get; set; }
        public int NewPlansThisMonth { get; set; }
    }
}
