using System;

namespace CyberCity.DTOs.Payments
{
    public class CreatePaymentLinkRequestDto
    {
        public string UserUid { get; set; }
        public string PlanUid { get; set; }
        // OrgUid removed - chỉ hỗ trợ thanh toán cá nhân
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
