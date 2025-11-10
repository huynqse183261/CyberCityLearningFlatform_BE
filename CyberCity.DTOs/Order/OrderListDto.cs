using System;

namespace CyberCity.DTOs.Order
{
    /// <summary>
    /// DTO cho danh sách đơn hàng (simplified)
    /// </summary>
    public class OrderListDto
    {
        public string Uid { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string PlanName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string ApprovalStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        
        /// <summary>
        /// Số lượng payment của order này
        /// </summary>
        public int PaymentCount { get; set; }
    }
}
