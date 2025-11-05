using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.DTOs.Payments;

namespace CyberCity.Application.Interface
{
    public interface IPaymentService
    {
        Task<PaymentLinkResponseDto> CreatePaymentLinkAsync(CreatePaymentLinkRequestDto request);
        Task<PaymentStatusDto> GetPaymentStatusAsync(long orderCode);
        Task<bool> VerifyWebhookSignatureAsync(string webhookUrl, string signature);
        Task<bool> CancelPaymentLinkAsync(long orderCode, string cancellationReason = null);
        Task HandlePaymentWebhookAsync(PaymentWebhookDto webhookData);
        Task<PaymentInvoiceDto> GetPaymentInvoiceAsync(string paymentUid);
        Task<List<PaymentHistoryDto>> GetPaymentHistoryAsync(string userUid);
        Task HandlePaymentCancelAsync(long orderCode);
    }
}
