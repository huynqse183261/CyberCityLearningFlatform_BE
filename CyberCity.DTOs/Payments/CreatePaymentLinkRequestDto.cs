using System;

namespace CyberCity.DTOs.Payments
{
    public class CreatePaymentLinkRequestDto
    {
        public string UserUid { get; set; }
        public string PlanUid { get; set; }
        // OrgUid removed - chỉ hỗ trợ thanh toán cá nhân
        // ReturnUrl và CancelUrl removed - Sepay không có redirect flow như PayOS
    }
}
