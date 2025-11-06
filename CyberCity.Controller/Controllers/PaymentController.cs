using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CyberCity.Application.Interface;
using CyberCity.DTOs.Payments;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text;

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
        /// </summary>
        /// <returns>Xác nhận đã nhận webhook</returns>
        [HttpPost("webhook")]
        [HttpPost("webhook/sepay")]
        [AllowAnonymous]
        public async Task<IActionResult> SepayWebhook()
        {
            try
            {
                // Đọc raw body từ request
                using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
                var payloadJson = await reader.ReadToEndAsync();
                
                // Lấy Authorization header
                var authorizationHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    // Thử lấy từ Apikey header (Sepay format)
                    authorizationHeader = Request.Headers["Apikey"].ToString();
                    if (!string.IsNullOrEmpty(authorizationHeader))
                    {
                        authorizationHeader = $"Apikey {authorizationHeader}";
                    }
                }

                var result = await _paymentService.ProcessSepayWebhookAsync(authorizationHeader, payloadJson);

                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Webhook processed successfully"
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Failed to process webhook"
                    });
                }
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
    }
}