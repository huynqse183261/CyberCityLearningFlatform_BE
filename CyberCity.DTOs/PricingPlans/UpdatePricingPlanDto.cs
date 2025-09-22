using System.ComponentModel.DataAnnotations;

namespace CyberCity.DTOs.PricingPlans
{
    public class UpdatePricingPlanDto
    {
        [StringLength(100)]
        public string PlanName { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal? Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 day")]
        public int? DurationDays { get; set; }

        [StringLength(1000)]
        public string Features { get; set; }
    }
}