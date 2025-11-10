using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CyberCity.Application.Interface;
using CyberCity.DTOs.Payments;
using CyberCity.DTOs.Order;
using CyberCity.Infrastructure;
using CyberCity.Doman.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CyberCity.Application.Implement
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly PaymentRepo _paymentRepo;
        private readonly OrderRepo _orderRepo;
        private readonly UserRepo _userRepo;
        private readonly PricingPlanRepo _pricingPlanRepo;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IConfiguration configuration,
            PaymentRepo paymentRepo,
            OrderRepo orderRepo,
            UserRepo userRepo,
            PricingPlanRepo pricingPlanRepo,
            ILogger<PaymentService> logger)
        {
            _configuration = configuration;
            _paymentRepo = paymentRepo;
            _orderRepo = orderRepo;
            _userRepo = userRepo;
            _pricingPlanRepo = pricingPlanRepo;
            _logger = logger;
        }

        public async Task<PaymentLinkResponseDto> CreatePaymentLinkAsync(CreatePaymentLinkRequestDto request)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(request.UserUid);
                if (user == null) throw new Exception($"User with UID {request.UserUid} not found");

                var plan = await _pricingPlanRepo.GetByIdAsync(request.PlanUid);
                if (plan == null) throw new Exception($"Pricing plan with UID {request.PlanUid} not found");

                var order = new Order
                {
                    Uid = Guid.NewGuid().ToString(),
                    UserUid = request.UserUid,
                    OrgUid = null,
                    PlanUid = request.PlanUid,
                    Amount = plan.Price,
                    PaymentStatus = "pending",
                    ApprovalStatus = "approved",
                    StartAt = null,
                    EndAt = null,
                    CreatedAt = DateTime.Now
                };
                await _orderRepo.CreateAsync(order);

                // Lấy cấu hình Sepay
                var bankCode = _configuration["Sepay:BankCode"];
                var accountNumber = _configuration["Sepay:AccountNumber"];
                
                if (string.IsNullOrWhiteSpace(bankCode) || string.IsNullOrWhiteSpace(accountNumber))
                {
                    throw new Exception("Thiếu cấu hình Sepay:BankCode hoặc Sepay:AccountNumber");
                }

                // Kiểm tra nếu có QR code PENDING chưa hết hạn cho order này (idempotent)
                var existingPayments = await _paymentRepo.GetAllAsync()
                    .Where(p => p.OrderUid == order.Uid && 
                               p.Status == "pending" && 
                               p.PaymentMethod == "SEPAY")
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                // Tái sử dụng payment còn hiệu lực (nếu có)
                var existingPayment = existingPayments.FirstOrDefault();
                if (existingPayment != null && existingPayment.TransactionCode != null)
                {
                    // Tạo lại QR URL từ TransactionCode (GatewayOrderCode)
                    var existingGatewayOrderCode = existingPayment.TransactionCode;
                    var existingAddInfo = $"CYBERCITY-{existingGatewayOrderCode}";
                    var qrImageUrl = GenerateSepayQrUrl(bankCode, accountNumber, plan.Price, existingAddInfo);

                    // Parse hex string (GUID part) thành long
                    var existingGuidPart = existingGatewayOrderCode.Split('-').LastOrDefault() ?? "0";
                    long existingOrderCode = 0;
                    try
                    {
                        // Thử parse hex string trước
                        existingOrderCode = Convert.ToInt64(existingGuidPart, 16);
                    }
                    catch
                    {
                        // Nếu không phải hex, thử parse như số thập phân
                        if (!long.TryParse(existingGuidPart, out existingOrderCode))
                        {
                            // Fallback: dùng hash code
                            existingOrderCode = Math.Abs(existingGuidPart.GetHashCode());
                        }
                    }

                    return new PaymentLinkResponseDto
                    {
                        Uid = existingPayment.Uid,
                        CheckoutUrl = qrImageUrl, // QR URL
                        QrCode = qrImageUrl, // QR URL
                        OrderCode = existingOrderCode,
                        Status = "pending",
                        Amount = plan.Price,
                        Description = $"{user.FullName}_{plan.PlanName}_{plan.DurationDays}days",
                        UserName = user.FullName,
                        PlanName = plan.PlanName
                    };
                }

                // Tạo mới GatewayOrderCode: ORD{OrderUid}-{GUID}
                var orderUidShort = order.Uid.Substring(0, Math.Min(8, order.Uid.Length));
                var gatewayOrderCode = $"ORD{orderUidShort}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
                var addInfo = $"CYBERCITY-{gatewayOrderCode}";

                // Tạo QR URL theo format Sepay: https://qr.sepay.vn/img?acc={accountNumber}&bank={bankCode}&amount={amount}&des={description}
                var qrUrl = GenerateSepayQrUrl(bankCode, accountNumber, plan.Price, addInfo);

                // Tạo payment record
                var payment = new Payment
                {
                    Uid = Guid.NewGuid().ToString(),
                    OrderUid = order.Uid,
                    PaymentMethod = "SEPAY",
                    TransactionCode = gatewayOrderCode, // Lưu GatewayOrderCode vào TransactionCode
                    Amount = plan.Price,
                    Currency = "VND",
                    Status = "pending",
                    CreatedAt = DateTime.Now
                };
                await _paymentRepo.CreateAsync(payment);

                _logger.LogInformation("Created Sepay QR payment - PaymentUid: {PaymentUid}, GatewayOrderCode: {GatewayOrderCode}, Amount: {Amount}",
                    payment.Uid, gatewayOrderCode, plan.Price);

                // Parse hex string (GUID part) thành long
                var guidPart = gatewayOrderCode.Split('-').LastOrDefault() ?? "0";
                long orderCode = 0;
                try
                {
                    // Parse hex string (GUID là hex) thành long
                    orderCode = Convert.ToInt64(guidPart, 16);
                }
                catch
                {
                    // Nếu không phải hex, thử parse như số thập phân
                    if (!long.TryParse(guidPart, out orderCode))
                    {
                        // Fallback: dùng hash code
                        orderCode = Math.Abs(guidPart.GetHashCode());
                    }
                }

                return new PaymentLinkResponseDto
                {
                    Uid = payment.Uid,
                    CheckoutUrl = qrUrl, // QR URL
                    QrCode = qrUrl, // QR URL
                    OrderCode = orderCode,
                    Status = "pending",
                    Amount = plan.Price,
                    Description = $"{user.FullName}_{plan.PlanName}_{plan.DurationDays}days",
                    UserName = user.FullName,
                    PlanName = plan.PlanName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Sepay payment link");
                throw new Exception($"Failed to create payment link: {ex.Message}", ex);
            }
        }

        private string GenerateSepayQrUrl(string bankCode, string accountNumber, decimal amount, string description)
        {
            // Format QR URL: https://qr.sepay.vn/img?acc={accountNumber}&bank={bankCode}&amount={amount}&des={description}
            var vndInt = (long)Math.Round(amount, 0, MidpointRounding.AwayFromZero);
            return $"https://qr.sepay.vn/img?acc={Uri.EscapeDataString(accountNumber)}&bank={Uri.EscapeDataString(bankCode)}&amount={vndInt}&des={Uri.EscapeDataString(description)}";
        }

        public async Task<PaymentStatusDto> GetPaymentStatusAsync(long orderCode)
        {
            try
            {
                // Map numeric orderCode back to hex GUID suffix (8 chars), case-insensitive
                // Ensure we pad with leading zeros to maintain 8-character match
                var hexSuffix = orderCode.ToString("x"); // lower-case hex
                if (hexSuffix.Length < 8) hexSuffix = hexSuffix.PadLeft(8, '0');

                // Try find by TransactionCode ending with the hex suffix (case-insensitive)
                // Use ToLower().EndsWith() to ensure EF can translate to SQL
                var query = _paymentRepo.GetAllAsync();
                var hexSuffixLower = hexSuffix.ToLowerInvariant();
                var payment = await query
                    .FirstOrDefaultAsync(p => p.TransactionCode != null &&
                                              p.TransactionCode.ToLower().EndsWith(hexSuffixLower));

                // Fallback: sometimes TransactionCode may store decimal or different formats
                if (payment == null)
                {
                    var decStr = orderCode.ToString();
                    payment = await query.FirstOrDefaultAsync(p => p.TransactionCode != null && p.TransactionCode.Contains(decStr));
                }

                if (payment == null)
                    throw new Exception($"Payment with order code {orderCode} not found");

                var amountPaid = (payment.Status == "completed" || payment.Status == "paid") ? payment.Amount : 0m;

                return new PaymentStatusDto
                {
                    OrderCode = orderCode,
                    Amount = payment.Amount,
                    AmountPaid = amountPaid.ToString(),
                    AmountRemaining = payment.Amount - amountPaid,
                    Status = payment.Status?.ToUpperInvariant() ?? "PENDING",
                    CreatedAt = payment.CreatedAt,
                    CanceledAt = (payment.Status == "cancelled" || payment.Status == "failed") ? payment.PaidAt : null,
                    CancellationReason = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get payment status for orderCode: {OrderCode}", orderCode);
                throw new Exception($"Failed to get payment status: {ex.Message}", ex);
            }
        }

        public async Task<bool> CancelPaymentLinkAsync(long orderCode, string cancellationReason = null)
        {
            try
            {
                // Tìm payment theo orderCode
                var payment = await _paymentRepo.GetAllAsync()
                    .FirstOrDefaultAsync(p => p.TransactionCode != null && 
                                             p.TransactionCode.Contains(orderCode.ToString()));

                if (payment == null)
                    throw new Exception($"Payment with order code {orderCode} not found");

                if (payment.Status == "pending")
                {
                    payment.Status = "cancelled";
                    await _paymentRepo.UpdateAsync(payment);

                    var order = await _orderRepo.GetByIdAsync(payment.OrderUid);
                    if (order != null && order.PaymentStatus == "pending")
                    {
                        order.PaymentStatus = "failed";
                        await _orderRepo.UpdateAsync(order);
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel payment link for orderCode: {OrderCode}", orderCode);
                throw new Exception($"Failed to cancel payment link: {ex.Message}", ex);
            }
        }

        public async Task HandlePaymentWebhookAsync(PaymentWebhookDto webhookData)
        {
            // Method này được giữ lại để tương thích với interface, nhưng sẽ được gọi từ ProcessSepayWebhookAsync
            // Webhook từ Sepay sẽ được xử lý riêng qua ProcessSepayWebhookAsync
            await Task.CompletedTask;
        }

        public async Task<bool> ProcessSepayWebhookAsync(string authorizationHeader, SepayWebhookDto webhookData)
        {
            try
            {
                _logger.LogInformation("[ProcessSepayWebhook] Starting - AuthHeader: {AuthHeader}, SepayId: {SepayId}", 
                    authorizationHeader, webhookData?.Id);

                if (webhookData == null)
                {
                    _logger.LogError("[ProcessSepayWebhook] Webhook data is null");
                    return false;
                }

                // Verify webhook token
                var expectedToken = _configuration["Sepay:WebhookToken"];
                _logger.LogInformation("[ProcessSepayWebhook] Expected token configured: {HasToken}", !string.IsNullOrEmpty(expectedToken));
                
                if (!string.IsNullOrEmpty(expectedToken))
                {
                    var expectedHeader = $"Apikey {expectedToken}";
                    if (!string.Equals(authorizationHeader, expectedHeader, StringComparison.Ordinal))
                    {
                        _logger.LogWarning("[ProcessSepayWebhook] Invalid authorization - Expected: {Expected}, Received: {Received}", 
                            expectedHeader, authorizationHeader);
                        return false;
                    }
                    _logger.LogInformation("[ProcessSepayWebhook] Token verification passed");
                }
                else
                {
                    _logger.LogWarning("[ProcessSepayWebhook] No webhook token configured - skipping verification");
                }

                // Extract data from webhook
                var sepayId = webhookData.Id;
                var amount = webhookData.TransferAmount;
                var description = webhookData.Content ?? webhookData.Description ?? string.Empty;
                var transactionRef = webhookData.ReferenceCode;
                var gateway = webhookData.Gateway ?? "Unknown";

                _logger.LogInformation("[ProcessSepayWebhook] Parsed data - SepayId: {SepayId}, Gateway: {Gateway}, Description: {Description}, TransactionRef: {TransactionRef}, Amount: {Amount}",
                    sepayId, gateway, description, transactionRef, amount);

                // Tìm payment theo GatewayOrderCode từ description
                string? gatewayOrderCode = null;
                string? guidPart = null;
                
                _logger.LogInformation("[ProcessSepayWebhook] Extracting gatewayOrderCode from description");
                
                if (!string.IsNullOrEmpty(description))
                {
                    // Format thực tế từ bank có thể là:
                    // - "CYBERCITY-ORD{8}-{8}"
                    // - "CYBERCITY ORD{8}{8}" (không dấu gạch, gộp 16 ký tự sau ORD)
                    // - "... CYBERCITYORD{16}- Ma GD ..."

                    // 1) Thử pattern chuẩn có 2 nhóm 8+ ký tự
                    var match = Regex.Match(description, @"CYBERCITY[-:\s]?ORD([A-Za-z0-9]{8,})[-_\s]?([A-Za-z0-9]{8,})", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        var orderPart = match.Groups[1].Value.Substring(0, Math.Min(8, match.Groups[1].Value.Length));
                        var guidPortion = match.Groups[2].Value.Substring(0, Math.Min(8, match.Groups[2].Value.Length));
                        gatewayOrderCode = $"ORD{orderPart}-{guidPortion}";
                        guidPart = guidPortion;
                        _logger.LogInformation("[ProcessSepayWebhook] Extracted (2-part) gatewayOrderCode: {GatewayOrderCode}, guidPart: {GuidPart}", gatewayOrderCode, guidPart);
                    }
                    else
                    {
                        // 2) Thử pattern gộp: CYBERCITYORD{16}
                        var matchMerged = Regex.Match(description, @"CYBERCITY\s*ORD([A-Za-z0-9]{16,})", RegexOptions.IgnoreCase);
                        if (!matchMerged.Success)
                        {
                            // cũng có thể viết liền "CYBERCITYORD{16}"
                            matchMerged = Regex.Match(description, @"CYBERCITYORD([A-Za-z0-9]{16,})", RegexOptions.IgnoreCase);
                        }
                        if (matchMerged.Success)
                        {
                            var sixteen = matchMerged.Groups[1].Value;
                            var orderPart = sixteen.Substring(0, Math.Min(8, sixteen.Length));
                            var guidPortion = sixteen.Length >= 16 ? sixteen.Substring(8, 8) : sixteen.Substring(Math.Min(8, sixteen.Length - 1));
                            gatewayOrderCode = $"ORD{orderPart}-{guidPortion}";
                            guidPart = guidPortion;
                            _logger.LogInformation("[ProcessSepayWebhook] Extracted (merged 16) gatewayOrderCode: {GatewayOrderCode}, guidPart: {GuidPart}", gatewayOrderCode, guidPart);
                        }
                        else
                        {
                            _logger.LogWarning("[ProcessSepayWebhook] Could not extract gatewayOrderCode from description: {Description}", description);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("[ProcessSepayWebhook] Description is empty");
                }

                // Tìm payment
                Payment? payment = null;
                
                _logger.LogInformation("[ProcessSepayWebhook] Searching for payment - Strategy 1: By gatewayOrderCode");
                if (!string.IsNullOrEmpty(gatewayOrderCode))
                {
                    payment = await _paymentRepo.GetAllAsync()
                        .FirstOrDefaultAsync(p => p.TransactionCode == gatewayOrderCode);
                    if (payment != null)
                    {
                        _logger.LogInformation("[ProcessSepayWebhook] Found payment by gatewayOrderCode: {PaymentUid}", payment.Uid);
                    }
                }

                // Tìm theo GUID suffix
                if (payment == null && !string.IsNullOrEmpty(guidPart))
                {
                    _logger.LogInformation("[ProcessSepayWebhook] Strategy 2: Searching by GUID suffix: {GuidPart}", guidPart);
                    var allPending = await _paymentRepo.GetAllAsync()
                        .Where(p => p.Status == "pending" && 
                                   p.PaymentMethod == "SEPAY" &&
                                   !string.IsNullOrEmpty(p.TransactionCode) &&
                                   p.TransactionCode.EndsWith(guidPart, StringComparison.OrdinalIgnoreCase))
                        .ToListAsync();
                    
                    _logger.LogInformation("[ProcessSepayWebhook] Found {Count} pending payments with matching GUID suffix", allPending.Count);
                    payment = allPending.FirstOrDefault();
                    if (payment != null)
                    {
                        _logger.LogInformation("[ProcessSepayWebhook] Selected payment: {PaymentUid}", payment.Uid);
                    }
                }

                // Tìm theo Sepay ID
                if (payment == null && sepayId > 0)
                {
                    _logger.LogInformation("[ProcessSepayWebhook] Strategy 3: Searching by Sepay ID: {SepayId}", sepayId);
                    var sepayIdStr = $"SEPAY-{sepayId}";
                    payment = await _paymentRepo.GetAllAsync()
                        .FirstOrDefaultAsync(p => p.TransactionCode == sepayIdStr);
                    if (payment != null)
                    {
                        _logger.LogInformation("[ProcessSepayWebhook] Found payment by Sepay ID: {PaymentUid}", payment.Uid);
                    }
                }

                if (payment == null)
                {
                    _logger.LogWarning("[ProcessSepayWebhook] Payment NOT FOUND - GatewayOrderCode: {GatewayOrderCode}, GuidPart: {GuidPart}, TransactionRef: {TransactionRef}, SepayId: {SepayId}",
                        gatewayOrderCode, guidPart, transactionRef, sepayId);
                    return false;
                }

                _logger.LogInformation("[ProcessSepayWebhook] Found payment {PaymentUid} with GatewayOrderCode: {GatewayOrderCode}",
                    payment.Uid, payment.TransactionCode);

                // Kiểm tra đã xử lý chưa (idempotency)
                if (payment.Status == "completed" || payment.Status == "paid")
                {
                    _logger.LogInformation("[ProcessSepayWebhook] Payment {PaymentUid} already processed - Status: {Status}", payment.Uid, payment.Status);
                    return true;
                }

                _logger.LogInformation("[ProcessSepayWebhook] Updating payment {PaymentUid} to completed", payment.Uid);

                // Cập nhật order trước (partial update)
                _logger.LogInformation("[ProcessSepayWebhook] Updating order {OrderUid}", payment.OrderUid);
                var affected = await _orderRepo.UpdatePaymentStatusAsync(payment.OrderUid, "paid");
                if (affected <= 0)
                {
                    _logger.LogWarning("[ProcessSepayWebhook] Order {OrderUid} not found or not updated", payment.OrderUid);
                    // Nếu order không tồn tại, không nên update payment
                    return false;
                }

                // Cập nhật payment (partial update) - dùng trạng thái hợp lệ theo DB (paid)
                await _paymentRepo.UpdateStatusAsync(payment.Uid, "paid", DateTime.Now);

                _logger.LogInformation("[ProcessSepayWebhook] Payment {PaymentUid} processed successfully", payment.Uid);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ProcessSepayWebhook] EXCEPTION: {Message}", ex.Message);
                throw; // Re-throw để controller bắt và log
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

                if (payment == null) throw new Exception($"Payment with UID {paymentUid} not found");

                var order = payment.OrderU;
                var user = order.UserU;
                var plan = order.PlanU;
                var org = order.Or;

                return new PaymentInvoiceDto
                {
                    PaymentUid = payment.Uid,
                    InvoiceNumber = $"INV-{payment.TransactionCode}",
                    InvoiceDate = payment.CreatedAt ?? DateTime.Now,
                    CustomerName = user?.FullName,
                    CustomerEmail = user?.Email,
                    CustomerPhone = "",
                    OrderUid = order.Uid,
                    PlanName = plan?.PlanName,
                    DurationDays = plan?.DurationDays ?? 0,
                    ServiceStartDate = order.StartAt,
                    ServiceEndDate = order.EndAt,
                    PaymentMethod = payment.PaymentMethod,
                    TransactionCode = payment.TransactionCode,
                    Amount = payment.Amount,
                    Currency = payment.Currency,
                    Status = payment.Status,
                    PaidAt = payment.PaidAt,
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
                var payment = await _paymentRepo.GetAllAsync()
                    .FirstOrDefaultAsync(p => p.TransactionCode == orderCode.ToString());

                if (payment == null)
                    throw new Exception($"Payment with order code {orderCode} not found");

                if (payment.Status == "pending")
                {
                    payment.Status = "failed";
                    await _paymentRepo.UpdateAsync(payment);

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

        public async Task<bool> VerifyWebhookSignatureAsync(string webhookUrl, string signature)
        {
            // Sepay sử dụng Apikey header để verify, không cần verify signature như PayOS
            // Method này được giữ lại để tương thích với interface
            await Task.CompletedTask;
            return true;
        }

        // ==================== ORDER MANAGEMENT ====================

        public async Task<List<OrderListDto>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _orderRepo.GetAllAsync()
                    .Include(o => o.UserU)
                    .Include(o => o.PlanU)
                    .Include(o => o.Payments)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                return orders.Select(o => new OrderListDto
                {
                    Uid = o.Uid,
                    UserName = o.UserU?.FullName ?? "N/A",
                    UserEmail = o.UserU?.Email ?? "N/A",
                    PlanName = o.PlanU?.PlanName ?? "N/A",
                    Amount = o.Amount,
                    PaymentStatus = o.PaymentStatus,
                    ApprovalStatus = o.ApprovalStatus,
                    CreatedAt = o.CreatedAt,
                    PaidAt = o.Payments?.FirstOrDefault(p => p.Status == "paid" || p.Status == "completed")?.PaidAt,
                    PaymentCount = o.Payments?.Count ?? 0
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all orders");
                throw new Exception($"Failed to get orders: {ex.Message}", ex);
            }
        }

        public async Task<List<OrderListDto>> GetOrdersByUserAsync(string userUid)
        {
            try
            {
                var orders = await _orderRepo.GetAllAsync()
                    .Include(o => o.UserU)
                    .Include(o => o.PlanU)
                    .Include(o => o.Payments)
                    .Where(o => o.UserUid == userUid)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                return orders.Select(o => new OrderListDto
                {
                    Uid = o.Uid,
                    UserName = o.UserU?.FullName ?? "N/A",
                    UserEmail = o.UserU?.Email ?? "N/A",
                    PlanName = o.PlanU?.PlanName ?? "N/A",
                    Amount = o.Amount,
                    PaymentStatus = o.PaymentStatus,
                    ApprovalStatus = o.ApprovalStatus,
                    CreatedAt = o.CreatedAt,
                    PaidAt = o.Payments?.FirstOrDefault(p => p.Status == "paid" || p.Status == "completed")?.PaidAt,
                    PaymentCount = o.Payments?.Count ?? 0
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get orders for user: {UserUid}", userUid);
                throw new Exception($"Failed to get orders: {ex.Message}", ex);
            }
        }

        public async Task<OrderDetailDto> GetOrderDetailAsync(string orderUid)
        {
            try
            {
                var order = await _orderRepo.GetAllAsync()
                    .Include(o => o.UserU)
                    .Include(o => o.Or)
                    .Include(o => o.PlanU)
                    .Include(o => o.Payments)
                    .FirstOrDefaultAsync(o => o.Uid == orderUid);

                if (order == null)
                    throw new Exception($"Order with UID {orderUid} not found");

                return new OrderDetailDto
                {
                    Uid = order.Uid,
                    UserUid = order.UserUid,
                    UserName = order.UserU?.FullName ?? "N/A",
                    UserEmail = order.UserU?.Email ?? "N/A",
                    OrgUid = order.OrgUid,
                    OrgName = order.Or?.OrgName,
                    PlanUid = order.PlanUid,
                    PlanName = order.PlanU?.PlanName ?? "N/A",
                    DurationDays = order.PlanU?.DurationDays ?? 0,
                    Amount = order.Amount,
                    PaymentStatus = order.PaymentStatus,
                    ApprovalStatus = order.ApprovalStatus,
                    StartAt = order.StartAt,
                    EndAt = order.EndAt,
                    CreatedAt = order.CreatedAt,
                    Payments = order.Payments?.Select(p => new OrderPaymentDto
                    {
                        Uid = p.Uid,
                        PaymentMethod = p.PaymentMethod,
                        TransactionCode = p.TransactionCode,
                        Amount = p.Amount,
                        Currency = p.Currency,
                        Status = p.Status,
                        PaidAt = p.PaidAt,
                        CreatedAt = p.CreatedAt
                    }).ToList() ?? new List<OrderPaymentDto>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get order detail for: {OrderUid}", orderUid);
                throw new Exception($"Failed to get order detail: {ex.Message}", ex);
            }
        }
    }
}
