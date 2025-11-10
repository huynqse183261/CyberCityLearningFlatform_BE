using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.DTOs.Payments;
using CyberCity.DTOs.Order;

namespace CyberCity.Application.Interface
{
    public interface IPaymentService
    {
        Task<PaymentLinkResponseDto> CreatePaymentLinkAsync(CreatePaymentLinkRequestDto request);
        Task<PaymentStatusDto> GetPaymentStatusAsync(long orderCode);
        Task<bool> VerifyWebhookSignatureAsync(string webhookUrl, string signature);
        Task<bool> CancelPaymentLinkAsync(long orderCode, string cancellationReason = null);
        Task HandlePaymentWebhookAsync(PaymentWebhookDto webhookData);
        
        /// <summary>
        /// Xử lý webhook từ Sepay với DTO đã parsed
        /// </summary>
        Task<bool> ProcessSepayWebhookAsync(string authorizationHeader, SepayWebhookDto webhookData);
        
        Task<PaymentInvoiceDto> GetPaymentInvoiceAsync(string paymentUid);
        Task<List<PaymentHistoryDto>> GetPaymentHistoryAsync(string userUid);
        Task HandlePaymentCancelAsync(long orderCode);
        
        // Order management
        Task<List<OrderListDto>> GetAllOrdersAsync();
        Task<List<OrderListDto>> GetOrdersByUserAsync(string userUid);
        Task<OrderDetailDto> GetOrderDetailAsync(string orderUid);
    }
}
