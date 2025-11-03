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
    }
}
