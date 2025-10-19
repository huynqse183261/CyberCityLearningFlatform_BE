using CyberCity.Application.Interface;
using CyberCity.DTOs.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CyberCity.Controller.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "admin")]
    public class AdminMessagesController : ControllerBase
    {
        private readonly IAdminMessageService _service;

        public AdminMessagesController(IAdminMessageService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách cuộc hội thoại
        /// </summary>
        [HttpGet("conversations")]
        public async Task<ActionResult<ConversationsListResponse>> GetConversations(
            [FromQuery] GetConversationsQuery query)
        {
            try
            {
                var result = await _service.GetConversationsAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy tin nhắn trong cuộc hội thoại
        /// </summary>
        [HttpGet("conversations/{conversationId}/messages")]
        public async Task<ActionResult<MessagesListResponse>> GetMessages(
            Guid conversationId,
            [FromQuery] GetMessagesQuery query)
        {
            try
            {
                var result = await _service.GetMessagesAsync(conversationId, query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Gửi tin nhắn phản hồi (Admin gửi)
        /// </summary>
        [HttpPost("conversations/{conversationId}/messages")]
        public async Task<ActionResult<SendMessageResponse>> SendMessage(
            Guid conversationId,
            [FromBody] SendMessageRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = GetCurrentUserId();
                var result = await _service.SendMessageAsync(conversationId, userId, request);
                
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Xóa tin nhắn
        /// </summary>
        [HttpDelete("messages/{messageId}")]
        public async Task<ActionResult<DeleteMessageResponse>> DeleteMessage(Guid messageId)
        {
            try
            {
                var result = await _service.DeleteMessageAsync(messageId);
                
                if (!result.IsSuccess)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Thống kê tổng quan
        /// </summary>
        [HttpGet("messages/stats")]
        public async Task<ActionResult<MessageStatsResponse>> GetStats()
        {
            try
            {
                var result = await _service.GetStatsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        private Guid GetCurrentUserId()
        {
            // Try to get from "sub" claim first (standard JWT)
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            
            // If not found, try "uid" claim
            if (string.IsNullOrEmpty(userIdClaim))
            {
                userIdClaim = User.FindFirst("uid")?.Value;
            }
            
            // If still not found, try NameIdentifier
            if (string.IsNullOrEmpty(userIdClaim))
            {
                userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }

            return userId;
        }
    }
}
