using System;

namespace CyberCity.DTOs.Subscriptions
{
    public class SubscriptionAccessDto
    {
        public bool HasAccess { get; set; }
        public bool CanViewAllModules { get; set; }
        public int MaxFreeModules { get; set; }
        public SubscriptionInfoDto? SubscriptionInfo { get; set; }
    }

    public class SubscriptionInfoDto
    {
        public bool Active { get; set; }
        public string OrderUid { get; set; }
        public string PlanUid { get; set; }
        public string PlanName { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public int? DaysRemaining { get; set; }
    }
}
