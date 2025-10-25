using System;

namespace CyberCity.DTOs.Order
{
    public class RecentOrderDto
    {
        public string Uid { get; set; }
        public string UserUid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PlanName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string ApprovalStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}