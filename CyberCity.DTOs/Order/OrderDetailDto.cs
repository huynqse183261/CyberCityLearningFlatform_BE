using System;

namespace CyberCity.DTOs.Order
{
    /// <summary>
    /// DTO cho thông tin chi tiết đơn hàng
    /// </summary>
    public class OrderDetailDto
    {
        public string Uid { get; set; }
        public string UserUid { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string? OrgUid { get; set; }
        public string? OrgName { get; set; }
        public string PlanUid { get; set; }
        public string PlanName { get; set; }
        public int DurationDays { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string ApprovalStatus { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        
        /// <summary>
        /// Danh sách các payment liên quan đến order này
        /// </summary>
        public List<OrderPaymentDto> Payments { get; set; } = new List<OrderPaymentDto>();
    }

    /// <summary>
    /// DTO cho payment trong order
    /// </summary>
    public class OrderPaymentDto
    {
        public string Uid { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionCode { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
