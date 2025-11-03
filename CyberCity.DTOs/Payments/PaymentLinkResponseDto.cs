using System;

namespace CyberCity.DTOs.Payments
{
    public class PaymentLinkResponseDto
    {
        public string Uid { get; set; }
        public string CheckoutUrl { get; set; }
        public string QrCode { get; set; }
        public long OrderCode { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public string PlanName { get; set; }
    }
}
