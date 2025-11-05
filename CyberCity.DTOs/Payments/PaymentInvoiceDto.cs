using System;

namespace CyberCity.DTOs.Payments
{
    public class PaymentInvoiceDto
    {
        public string PaymentUid { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        
        // Customer Info
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        
        // Order Info
        public string OrderUid { get; set; }
        public string PlanName { get; set; }
        public int DurationDays { get; set; }
        public DateTime? ServiceStartDate { get; set; }
        public DateTime? ServiceEndDate { get; set; }
        
        // Payment Info
        public string PaymentMethod { get; set; }
        public string TransactionCode { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public DateTime? PaidAt { get; set; }
        
        // Organization Info (if applicable)
        public string OrganizationName { get; set; }
        public string OrganizationCode { get; set; }
    }
}
