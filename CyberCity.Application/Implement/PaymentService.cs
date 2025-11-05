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
using System.Reflection;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Exceptions;

namespace CyberCity.Application.Implement
{
    public class PaymentService : IPaymentService
    {
        private PayOSClient? _payOSClient;
        private readonly IConfiguration _configuration;
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
            _configuration = configuration;
            _paymentRepo = paymentRepo;
            _orderRepo = orderRepo;
            _userRepo = userRepo;
            _pricingPlanRepo = pricingPlanRepo;
        }

        // Lazy initialization client cho PayOS v2 SDK
        private PayOSClient GetPayOSClient()
        {
            if (_payOSClient != null)
                return _payOSClient;

            var clientId = _configuration["PayOS:ClientId"] ?? Environment.GetEnvironmentVariable("PAYOS_CLIENT_ID");
            var apiKey = _configuration["PayOS:ApiKey"] ?? Environment.GetEnvironmentVariable("PAYOS_API_KEY");
            var checksumKey = _configuration["PayOS:ChecksumKey"] ?? Environment.GetEnvironmentVariable("PAYOS_CHECKSUM_KEY");
            var baseUrl = _configuration["PayOS:BaseUrl"] ?? Environment.GetEnvironmentVariable("PAYOS_BASEURL") ?? "https://api-merchant.payos.vn";


            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(checksumKey))
                throw new Exception("PayOS credentials missing.");

            var options = new PayOSOptions
            {
                ClientId = clientId,
                ApiKey = apiKey,
                ChecksumKey = checksumKey,
                BaseUrl = baseUrl
            };

            _payOSClient = new PayOSClient(options);
            return _payOSClient;
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

                // Tạo order mới (cá nhân - không cần org)
                var order = new Order
                {
                    Uid = Guid.NewGuid().ToString(),
                    UserUid = request.UserUid,
                    OrgUid = null, // Luôn là null cho thanh toán cá nhân
                    PlanUid = request.PlanUid,
                    Amount = plan.Price, // Lấy giá từ pricing plan
                    PaymentStatus = "pending",
                    ApprovalStatus = "approved", // Cá nhân luôn tự động approved
                    StartAt = null,
                    EndAt = null,
                    CreatedAt = DateTime.Now
                };

                await _orderRepo.CreateAsync(order);

                // Generate unique order code (PayOS requires a long integer)
                var orderCode = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                // Tạo mô tả thanh toán kết hợp thông tin user và plan
                var description = $"{user.FullName} - {plan.PlanName} ({plan.DurationDays} ngày)";

                // Tạo danh sách items (v2 SDK)
                var items = new List<PaymentLinkItem>
                {
                    new PaymentLinkItem
                    {
                        Name = plan.PlanName,
                        Quantity = 1,
                        Price = checked((int)plan.Price)
                    }
                };

                // Validate callback URLs (bắt buộc phải có từ FE)
                if (string.IsNullOrWhiteSpace(request.CancelUrl))
                    throw new Exception("CancelUrl is required");
                if (string.IsNullOrWhiteSpace(request.ReturnUrl))
                    throw new Exception("ReturnUrl is required");

                var cancelUrl = request.CancelUrl;
                var returnUrl = request.ReturnUrl;

                // Tạo payment request (v2 SDK)
                var client = GetPayOSClient();
                var paymentRequest = new CreatePaymentLinkRequest
                {
                    OrderCode = orderCode,
                    Amount = checked((int)plan.Price),
                    Description = description,
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl,
                    Items = items
                };

                // 🔍 DEBUG: Log request trước khi gửi PayOS
                Console.WriteLine($"[PayOS Request Debug]");
                Console.WriteLine($"  OrderCode: {paymentRequest.OrderCode} (type: {paymentRequest.OrderCode.GetType().Name})");
                Console.WriteLine($"  Amount: {paymentRequest.Amount} (type: {paymentRequest.Amount.GetType().Name})");
                Console.WriteLine($"  Description: {paymentRequest.Description}");
                Console.WriteLine($"  ReturnUrl: {paymentRequest.ReturnUrl}");
                Console.WriteLine($"  CancelUrl: {paymentRequest.CancelUrl}");
                Console.WriteLine($"  Items: [{string.Join(", ", paymentRequest.Items.Select(i => $"{{Name:{i.Name}, Qty:{i.Quantity}, Price:{i.Price}}}"))}]");

                CreatePaymentLinkResponse createPaymentResult;
                try
                {
                    createPaymentResult = await client.PaymentRequests.CreateAsync(paymentRequest);
                }
                catch (ApiException apiEx)
                {
                    Console.WriteLine($"[PayOS Error] StatusCode={apiEx.StatusCode}, ErrorCode={apiEx.ErrorCode}, Message={apiEx.Message}");
                    throw new Exception($"PayOS API error ({apiEx.StatusCode}/{apiEx.ErrorCode}): {apiEx.Message}");
                }

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
                    CreatedAt = DateTime.Now
                };

                await _paymentRepo.CreateAsync(payment);

                return new PaymentLinkResponseDto
                {
                    Uid = payment.Uid,
                    CheckoutUrl = createPaymentResult.CheckoutUrl,
                    QrCode = createPaymentResult.QrCode,
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
                var client = GetPayOSClient();
                try
                {
                    var paymentLinkInfo = await client.PaymentRequests.GetAsync(orderCode);

                    // Normalize types from SDK to our DTO types
                    var amount = Convert.ToDecimal(paymentLinkInfo.Amount);
                    var amountPaidStr = paymentLinkInfo.AmountPaid.ToString();
                    var amountPaid = Convert.ToDecimal(paymentLinkInfo.AmountPaid);
                    DateTime? createdAt = null;
                    DateTime? canceledAt = null;
                    if (paymentLinkInfo.CreatedAt != null && DateTime.TryParse(paymentLinkInfo.CreatedAt.ToString(), out var ca))
                        createdAt = ca;
                    if (paymentLinkInfo.CanceledAt != null && DateTime.TryParse(paymentLinkInfo.CanceledAt.ToString(), out var cna))
                        canceledAt = cna;

                    return new PaymentStatusDto
                    {
                        OrderCode = orderCode,
                        Amount = amount,
                        AmountPaid = amountPaidStr,
                        AmountRemaining = amount - amountPaid,
                        Status = paymentLinkInfo.Status.ToString().ToUpperInvariant(),
                        CreatedAt = createdAt,
                        CanceledAt = canceledAt,
                        CancellationReason = paymentLinkInfo.CancellationReason
                    };
                }
                catch (ApiException apiEx)
                {
                    throw new Exception($"PayOS API error ({apiEx.StatusCode}/{apiEx.ErrorCode}): {apiEx.Message}");
                }
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
                // In v2 SDK, verification expects full webhook body; our current method is limited.
                // We'll attempt a confirm call to ensure webhook is registered.
                var client = GetPayOSClient();
                var confirm = await client.Webhooks.ConfirmAsync(webhookUrl);
                return confirm != null;
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
        var client = GetPayOSClient();
                try
                {
                    var result = await client.PaymentRequests.CancelAsync(orderCode, cancellationReason);

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
                catch (ApiException apiEx)
                {
                    throw new Exception($"PayOS API error ({apiEx.StatusCode}/{apiEx.ErrorCode}): {apiEx.Message}");
                }
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
                    payment.PaidAt = DateTime.Now;

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

        public async Task<PaymentInvoiceDto> GetPaymentInvoiceAsync(string paymentUid)
        {
            try
            {
                var payment = await _paymentRepo.GetAllAsync()
                    .Include(p => p.OrderU)
                        .ThenInclude(o => o.UserU)
                    .Include(p => p.OrderU)
                        .ThenInclude(o => o.PlanU)
                    .Include(p => p.OrderU)
                        .ThenInclude(o => o.Or)
                    .FirstOrDefaultAsync(p => p.Uid == paymentUid);

                if (payment == null)
                    throw new Exception($"Payment with UID {paymentUid} not found");

                var order = payment.OrderU;
                var user = order.UserU;
                var plan = order.PlanU;
                var org = order.Or;

                return new PaymentInvoiceDto
                {
                    PaymentUid = payment.Uid,
                    InvoiceNumber = $"INV-{payment.TransactionCode}",
                    InvoiceDate = payment.CreatedAt ?? DateTime.Now,

                    // Customer Info
                    CustomerName = user?.FullName,
                    CustomerEmail = user?.Email,
                    CustomerPhone = "",

                    // Order Info
                    OrderUid = order.Uid,
                    PlanName = plan?.PlanName,
                    DurationDays = plan?.DurationDays ?? 0,
                    ServiceStartDate = order.StartAt,
                    ServiceEndDate = order.EndAt,

                    // Payment Info
                    PaymentMethod = payment.PaymentMethod,
                    TransactionCode = payment.TransactionCode,
                    Amount = payment.Amount,
                    Currency = payment.Currency,
                    Status = payment.Status,
                    PaidAt = payment.PaidAt,

                    // Organization Info
                    OrganizationName = org?.OrgName,
                    OrganizationCode = ""
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get payment invoice: {ex.Message}", ex);
            }
        }

        public async Task<List<PaymentHistoryDto>> GetPaymentHistoryAsync(string userUid)
        {
            try
            {
                var payments = await _paymentRepo.GetAllAsync()
                    .Include(p => p.OrderU)
                        .ThenInclude(o => o.PlanU)
                    .Where(p => p.OrderU.UserUid == userUid)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return payments.Select(p => new PaymentHistoryDto
                {
                    Uid = p.Uid,
                    OrderId = p.OrderUid,
                    Amount = p.Amount,
                    Currency = p.Currency,
                    PaymentMethod = p.PaymentMethod,
                    Status = p.Status,
                    Description = "",
                    TransactionId = p.TransactionCode ?? "",
                    CreatedAt = p.CreatedAt ?? DateTime.Now,
                    CompletedAt = p.PaidAt
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get payment history: {ex.Message}", ex);
            }
        }

        public async Task HandlePaymentCancelAsync(long orderCode)
        {
            try
            {
                // Tìm payment theo transaction code (orderCode)
                var payment = _paymentRepo.GetAll()
                    .FirstOrDefault(p => p.TransactionCode == orderCode.ToString());

                if (payment == null)
                {
                    throw new Exception($"Payment with order code {orderCode} not found");
                }

                // Chỉ cập nhật nếu đang ở trạng thái pending
                if (payment.Status == "pending")
                {
                    payment.Status = "failed";
                    await _paymentRepo.UpdateAsync(payment);

                    // Cập nhật order status
                    var order = await _orderRepo.GetByIdAsync(payment.OrderUid);
                    if (order != null && order.PaymentStatus == "pending")
                    {
                        order.PaymentStatus = "failed";
                        await _orderRepo.UpdateAsync(order);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to handle payment cancel: {ex.Message}", ex);
            }
        }
    }
}

