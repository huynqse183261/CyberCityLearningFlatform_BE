using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CyberCity.Application.Interface;
using CyberCity.DTOs.Payments;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Linq;

namespace CyberCity.Controller.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Tạo link thanh toán Sepay (QR Code)
        /// </summary>
        /// <param name="request">Thông tin yêu cầu thanh toán</param>
        /// <returns>Link thanh toán và QR code</returns>
        [HttpPost("create-payment-link")]
        [Authorize]
        public async Task<IActionResult> CreatePaymentLink([FromBody] CreatePaymentLinkRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _paymentService.CreatePaymentLinkAsync(request);
                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Tạo link thanh toán thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy trạng thái thanh toán
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng</param>
        /// <returns>Thông tin trạng thái thanh toán</returns>
        [HttpGet("status/{orderCode}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentStatus(long orderCode)
        {
            try
            {
                var result = await _paymentService.GetPaymentStatusAsync(orderCode);
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Webhook endpoint để nhận thông báo từ Sepay khi thanh toán thành công
        /// Endpoint này hỗ trợ cả model binding và raw JSON parsing
        /// </summary>
        /// <param name="webhookData">Dữ liệu webhook từ Sepay (auto-bind từ JSON)</param>
        /// <returns>Xác nhận đã nhận webhook</returns>
        [HttpPost("webhook")]
        [HttpPost("webhook/sepay")]
        [AllowAnonymous]
        public async Task<IActionResult> SepayWebhook([FromBody] SepayWebhookDto webhookData)
        {
            try
            {
                // Log headers để debug
                var allHeaders = string.Join(", ", Request.Headers.Select(h => $"{h.Key}: {h.Value}"));
                Console.WriteLine($"[Webhook] Headers: {allHeaders}");
                
                // Lấy Authorization/Token header
                var authorizationHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    // Thử lấy từ Apikey header (Sepay format)
                    authorizationHeader = Request.Headers["Apikey"].ToString();
                    if (string.IsNullOrEmpty(authorizationHeader))
                    {
                        // Thử lấy từ token header
                        authorizationHeader = Request.Headers["token"].ToString();
                    }
                    
                    if (!string.IsNullOrEmpty(authorizationHeader) && !authorizationHeader.StartsWith("Apikey "))
                    {
                        authorizationHeader = $"Apikey {authorizationHeader}";
                    }
                }

                Console.WriteLine($"[Webhook] Authorization: {authorizationHeader}");
                
                // Validate webhook data
                if (webhookData == null)
                {
                    Console.WriteLine("[Webhook] ERROR: Webhook data is null (model binding failed)");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid webhook payload - model binding failed",
                        error = "NULL_DATA",
                        hint = "Check if JSON keys match the DTO properties (case-sensitive)"
                    });
                }

                Console.WriteLine($"[Webhook] Received - Id: {webhookData.Id}, Amount: {webhookData.TransferAmount}, Gateway: {webhookData.Gateway}, Content: {webhookData.Content}");

                var result = await _paymentService.ProcessSepayWebhookAsync(authorizationHeader, webhookData);

                if (result)
                {
                    Console.WriteLine("[Webhook] SUCCESS");
                    return Ok(new
                    {
                        success = true,
                        message = "Webhook processed successfully"
                    });
                }
                else
                {
                    Console.WriteLine("[Webhook] FAILED: Payment not found or already processed");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Failed to process webhook - payment not found or invalid",
                        error = "PROCESSING_FAILED"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Webhook] EXCEPTION: {ex.Message}");
                Console.WriteLine($"[Webhook] StackTrace: {ex.StackTrace}");
                var inner1 = ex.InnerException?.Message;
                var inner2 = ex.InnerException?.InnerException?.Message;
                if (!string.IsNullOrEmpty(inner1)) Console.WriteLine($"[Webhook] InnerException: {inner1}");
                if (!string.IsNullOrEmpty(inner2)) Console.WriteLine($"[Webhook] InnerMost: {inner2}");
                
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    error = "EXCEPTION",
                    details = new
                    {
                        exceptionType = ex.GetType().Name,
                        stackTrace = ex.StackTrace?.Split('\n').FirstOrDefault(),
                        innerException = inner1,
                        innerMost = inner2
                    }
                });
            }
        }

        /// <summary>
        /// Hủy link thanh toán
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng</param>
        /// <param name="reason">Lý do hủy</param>
        /// <returns>Kết quả hủy thanh toán</returns>
        [HttpPost("cancel/{orderCode}")]
        [Authorize]
        public async Task<IActionResult> CancelPaymentLink(long orderCode, [FromQuery] string reason = null)
        {
            try
            {
                var result = await _paymentService.CancelPaymentLinkAsync(orderCode, reason);
                
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Hủy thanh toán thành công"
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = "Không thể hủy thanh toán"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy thông tin hóa đơn chi tiết
        /// </summary>
        /// <param name="paymentUid">UID của payment</param>
        /// <returns>Thông tin hóa đơn đầy đủ</returns>
        [HttpGet("invoice/{paymentUid}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentInvoice(string paymentUid)
        {
            try
            {
                var invoice = await _paymentService.GetPaymentInvoiceAsync(paymentUid);
                return Ok(new
                {
                    success = true,
                    data = invoice
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy lịch sử thanh toán của user
        /// </summary>
        /// <param name="userUid">UID của user</param>
        /// <returns>Danh sách lịch sử thanh toán</returns>
        [HttpGet("history/{userUid}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentHistory(string userUid)
        {
            try
            {
                var history = await _paymentService.GetPaymentHistoryAsync(userUid);
                return Ok(new
                {
                    success = true,
                    data = history
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // ==================== ORDER MANAGEMENT ====================

        /// <summary>
        /// Lấy danh sách tất cả đơn hàng (Admin only)
        /// </summary>
        /// <returns>Danh sách tất cả đơn hàng</returns>
        [HttpGet("orders")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _paymentService.GetAllOrdersAsync();
                return Ok(new
                {
                    success = true,
                    data = orders,
                    total = orders.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy danh sách đơn hàng của user
        /// </summary>
        /// <param name="userUid">UID của user</param>
        /// <returns>Danh sách đơn hàng của user</returns>
        [HttpGet("orders/user/{userUid}")]
        [Authorize]
        public async Task<IActionResult> GetOrdersByUser(string userUid)
        {
            try
            {
                var orders = await _paymentService.GetOrdersByUserAsync(userUid);
                return Ok(new
                {
                    success = true,
                    data = orders,
                    total = orders.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy chi tiết đơn hàng
        /// </summary>
        /// <param name="orderUid">UID của order</param>
        /// <returns>Thông tin chi tiết đơn hàng bao gồm payments</returns>
        [HttpGet("order/{orderUid}")]
        [Authorize]
        public async Task<IActionResult> GetOrderDetail(string orderUid)
        {
            try
            {
                var order = await _paymentService.GetOrderDetailAsync(orderUid);
                return Ok(new
                {
                    success = true,
                    data = order
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}