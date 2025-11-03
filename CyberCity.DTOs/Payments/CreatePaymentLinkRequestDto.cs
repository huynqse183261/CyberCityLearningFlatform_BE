using System;

namespace CyberCity.DTOs.Payments
{
    public class CreatePaymentLinkRequestDto
    {
        public string UserUid { get; set; }
        public string PlanUid { get; set; }
        public string? OrgUid { get; set; } // Optional - for organization plans
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
