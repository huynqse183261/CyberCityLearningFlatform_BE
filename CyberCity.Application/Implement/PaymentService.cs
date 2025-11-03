using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.Application.Interface;
using CyberCity.DTOs.Payments;
using CyberCity.Infrastructure;
using CyberCity.Doman.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace CyberCity.Application.Implement
{
    public class PaymentService : IPaymentService
    {
        private readonly dynamic _payOS;
        private readonly PaymentRepo _paymentRepo;
        private readonly OrderRepo _orderRepo;
        private readonly UserRepo _userRepo;
        private readonly PricingPlanRepo _pricingPlanRepo;

        public PaymentService(
            IConfiguration configuration, 
            PaymentRepo paymentRepo, 
            OrderRepo orderRepo,
            UserRepo userRepo,
            PricingPlanRepo pricingPlanRepo)
        {
            var clientId = configuration["PayOS:ClientId"];
            var apiKey = configuration["PayOS:ApiKey"];
            var checksumKey = configuration["PayOS:ChecksumKey"];

            // Sử dụng reflection để tạo instance PayOS
            var payOSType = Type.GetType("PayOS.PayOS, PayOS");
            if (payOSType != null)
            {
                _payOS = Activator.CreateInstance(payOSType, clientId, apiKey, checksumKey);
            }
            else
            {
                throw new Exception("PayOS library not found");
            }
            
            _paymentRepo = paymentRepo;
            _orderRepo = orderRepo;
            _userRepo = userRepo;
            _pricingPlanRepo = pricingPlanRepo;
        }

        public async Task<PaymentLinkResponseDto> CreatePaymentLinkAsync(CreatePaymentLinkRequestDto request)
        {
            try
            {
                // Lấy thông tin người dùng
                var user = await _userRepo.GetByIdAsync(request.UserUid);
                if (user == null)
                    throw new Exception($"User with UID {request.UserUid} not found");

                // Lấy thông tin gói dịch vụ (pricing plan)
                var plan = await _pricingPlanRepo.GetByIdAsync(request.PlanUid);
                if (plan == null)
                    throw new Exception($"Pricing plan with UID {request.PlanUid} not found");

                // Tạo order mới
                var order = new Order
                {
                    Uid = Guid.NewGuid().ToString(),
                    UserUid = request.UserUid,
                    OrgUid = request.OrgUid,
                    PlanUid = request.PlanUid,
                    Amount = plan.Price, // Lấy giá từ pricing plan
                    PaymentStatus = "pending",
                    ApprovalStatus = string.IsNullOrEmpty(request.OrgUid) ? "approved" : "pending",
                    StartAt = null,
                    EndAt = null,
                    CreatedAt = DateTime.UtcNow
                };

                await _orderRepo.CreateAsync(order);

                // Generate unique order code (PayOS requires a long integer)
                var orderCode = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                // Tạo mô tả thanh toán kết hợp thông tin user và plan
                var description = $"{user.FullName} - {plan.PlanName} ({plan.DurationDays} ngày)";

                // Create ItemData using reflection
                var itemDataType = Type.GetType("PayOS.ItemData, PayOS");
                var itemData = Activator.CreateInstance(itemDataType, plan.PlanName, 1, (int)plan.Price);
                var items = new List<object> { itemData };

                // Create PaymentData using reflection
                var paymentDataType = Type.GetType("PayOS.PaymentData, PayOS");
                var paymentData = Activator.CreateInstance(paymentDataType,
                    orderCode,
                    (int)plan.Price,
                    description,
                    items,
                    request.CancelUrl ?? "http://localhost:5173/payment/cancel",
                    request.ReturnUrl ?? "http://localhost:5173/payment/success"
                );

                // Create payment link via PayOS
                var createPaymentResult = await _payOS.createPaymentLink(paymentData);

                // Save payment record to database
                var payment = new Payment
                {
                    Uid = Guid.NewGuid().ToString(),
                    OrderUid = order.Uid,
                    PaymentMethod = "PayOS",
                    TransactionCode = orderCode.ToString(),
                    Amount = plan.Price,
                    Currency = "VND",
                    Status = "pending",
                    CreatedAt = DateTime.UtcNow
                };

                await _paymentRepo.CreateAsync(payment);

                return new PaymentLinkResponseDto
                {
                    Uid = payment.Uid,
                    CheckoutUrl = createPaymentResult.checkoutUrl,
                    QrCode = createPaymentResult.qrCode,
                    OrderCode = orderCode,
                    Status = "pending",
                    Amount = plan.Price,
                    Description = description,
                    UserName = user.FullName,
                    PlanName = plan.PlanName
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create payment link: {ex.Message}", ex);
            }
        }

        public async Task<PaymentStatusDto> GetPaymentStatusAsync(long orderCode)
        {
            try
            {
                var paymentLinkInfo = await _payOS.getPaymentLinkInformation(orderCode);

                return new PaymentStatusDto
                {
                    OrderCode = orderCode,
                    Amount = paymentLinkInfo.amount,
                    AmountPaid = paymentLinkInfo.amountPaid.ToString(),
                    AmountRemaining = paymentLinkInfo.amount - int.Parse(paymentLinkInfo.amountPaid),
                    Status = paymentLinkInfo.status,
                    CreatedAt = paymentLinkInfo.createdAt != null ? DateTimeOffset.Parse(paymentLinkInfo.createdAt).DateTime : null,
                    CanceledAt = paymentLinkInfo.canceledAt != null ? DateTimeOffset.Parse(paymentLinkInfo.canceledAt).DateTime : null,
                    CancellationReason = paymentLinkInfo.cancellationReason
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get payment status: {ex.Message}", ex);
            }
        }

        public async Task<bool> VerifyWebhookSignatureAsync(string webhookUrl, string signature)
        {
            try
            {
                var webhookTypeType = Type.GetType("PayOS.WebhookType, PayOS");
                var webhookType = Activator.CreateInstance(webhookTypeType, webhookUrl, signature);
                var webhookData = _payOS.verifyPaymentWebhookData(webhookType);
                return webhookData != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CancelPaymentLinkAsync(long orderCode, string cancellationReason = null)
        {
            try
            {
                var result = await _payOS.cancelPaymentLink(orderCode, cancellationReason);
                
                // Update payment status in database
                var payment = await _paymentRepo.GetAllAsync()
                    .FirstOrDefaultAsync(p => p.TransactionCode == orderCode.ToString());

                if (payment != null)
                {
                    payment.Status = "cancelled";
                    await _paymentRepo.UpdateAsync(payment);
                }

                return result != null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to cancel payment link: {ex.Message}", ex);
            }
        }

        public async Task HandlePaymentWebhookAsync(PaymentWebhookDto webhookData)
        {
            try
            {
                // Find payment by transaction code (order code)
                var payment = await _paymentRepo.GetAllAsync()
                    .FirstOrDefaultAsync(p => p.TransactionCode == webhookData.OrderCode.ToString());

                if (payment == null)
                {
                    throw new Exception($"Payment with order code {webhookData.OrderCode} not found");
                }

                // Update payment status
                if (webhookData.Code == "00")
                {
                    payment.Status = "completed";
                    payment.PaidAt = DateTime.UtcNow;

                    // Update order status
                    var order = await _orderRepo.GetByIdAsync(payment.OrderUid);
                    if (order != null)
                    {
                        order.PaymentStatus = "paid";
                        await _orderRepo.UpdateAsync(order);
                    }
                }
                else
                {
                    payment.Status = "failed";
                }

                await _paymentRepo.UpdateAsync(payment);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to handle payment webhook: {ex.Message}", ex);
            }
        }
    }
}
