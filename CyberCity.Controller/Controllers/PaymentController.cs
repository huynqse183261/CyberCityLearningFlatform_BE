using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CyberCity.Application.Interface;
using CyberCity.DTOs.Payments;
using System;
using System.Threading.Tasks;

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
        /// Tạo link thanh toán PayOS
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
        /// <param name="orderCode">Mã đơn hàng PayOS</param>
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
        /// Webhook endpoint để nhận thông báo từ PayOS khi thanh toán thành công
        /// </summary>
        /// <param name="webhookData">Dữ liệu webhook từ PayOS</param>
        /// <returns>Xác nhận đã nhận webhook</returns>
        [HttpPost("webhook")]
        public async Task<IActionResult> PaymentWebhook([FromBody] PaymentWebhookDto webhookData)
        {
            try
            {
                // Verify webhook signature if needed
                var signature = Request.Headers["X-PayOS-Signature"].ToString();
                
                if (!string.IsNullOrEmpty(signature))
                {
                    var webhookUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
                    var isValid = await _paymentService.VerifyWebhookSignatureAsync(webhookUrl, signature);
                    
                    if (!isValid)
                    {
                        return Unauthorized(new { message = "Invalid webhook signature" });
                    }
                }

                await _paymentService.HandlePaymentWebhookAsync(webhookData);

                return Ok(new
                {
                    success = true,
                    message = "Webhook processed successfully"
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
        /// Hủy link thanh toán
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng PayOS</param>
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

        /// <summary>
        /// Callback khi user cancel thanh toán trên PayOS
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng</param>
        /// <returns>Thông báo hủy thanh toán</returns>
        [HttpGet("cancel-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> HandleCancelCallback([FromQuery] long orderCode)
        {
            try
            {
                await _paymentService.HandlePaymentCancelAsync(orderCode);
                
                // Redirect về frontend với message
                return Redirect($"{Request.Scheme}://localhost:5173/payment/cancelled?orderCode={orderCode}");
            }
            catch (Exception ex)
            {
                return Redirect($"{Request.Scheme}://localhost:5173/payment/error?message={ex.Message}");
            }
        }

        /// <summary>
        /// Callback khi user quay lại sau thanh toán (success hoặc không thanh toán)
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng</param>
        /// <returns>Redirect về frontend với trạng thái</returns>
        [HttpGet("return-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> HandleReturnCallback([FromQuery] long orderCode)
        {
            try
            {
                // Kiểm tra trạng thái từ PayOS
                var status = await _paymentService.GetPaymentStatusAsync(orderCode);
                
                if (status.Status == "PAID")
                {
                    return Redirect($"{Request.Scheme}://localhost:5173/payment/success?orderCode={orderCode}");
                }
                else
                {
                    // Nếu chưa thanh toán → Đánh dấu failed
                    await _paymentService.HandlePaymentCancelAsync(orderCode);
                    return Redirect($"{Request.Scheme}://localhost:5173/payment/cancelled?orderCode={orderCode}");
                }
            }
            catch (Exception ex)
            {
                return Redirect($"{Request.Scheme}://localhost:5173/payment/error?message={ex.Message}");
            }
        }
    }
}