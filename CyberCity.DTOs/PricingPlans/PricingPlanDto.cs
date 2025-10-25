using System;

namespace CyberCity.DTOs.PricingPlans
{
    public class PricingPlanDto
    {
        public string Uid { get; set; }
        public string PlanName { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public string Features { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int OrderCount { get; set; } // Số lượng orders sử dụng plan này
    }
}