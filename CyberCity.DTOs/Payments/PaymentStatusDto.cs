using System;

namespace CyberCity.DTOs.Payments
{
    public class PaymentStatusDto
    {
        public string Uid { get; set; }
        public long OrderCode { get; set; }
        public decimal Amount { get; set; }
        public string AmountPaid { get; set; }
        public decimal AmountRemaining { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? CanceledAt { get; set; }
        public string CancellationReason { get; set; }
    }
}
