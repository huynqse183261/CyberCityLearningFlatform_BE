using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.DTOs;
using CyberCity.DTOs.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using CyberCity.Controller.Hubs;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CyberCity.Controller.Controllers
{
    [Route("api/conversations")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(IMessageService messageService, IMapper mapper, IHubContext<ChatHub> hubContext)
        {
            _messageService = messageService;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        private string GetCurrentUserId()
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
            
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("Invalid user token");
            }
            return userIdClaim;
        }

        /// <summary>
        /// GET /api/conversations/{id}/messages - Tin nhắn trong cuộc trò chuyện
        /// </summary>
        [HttpGet("{id}/messages")]
        public async Task<ActionResult<PagedResult<MessageDto>>> GetMessages(
            string id, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var messages = await _messageService.GetMessagesByConversationIdAsync(id, currentUserId, pageNumber, pageSize);
                return Ok(messages);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You are not a member of this conversation");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/conversations/{id}/messages - Gửi tin nhắn (có thể dùng REST API hoặc SignalR)
        /// </summary>
        [HttpPost("{id}/messages")]
        public async Task<ActionResult<MessageDto>> SendMessage(string id, [FromBody] CreateMessageDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var currentUserId = GetCurrentUserId();
                var message = await _messageService.SendMessageAsync(id, createDto, currentUserId);
                
                // Send real-time notification to all conversation members via SignalR
                await _hubContext.Clients.Group($"conversation_{id}").SendAsync("ReceiveMessage", message);
                
                return CreatedAtAction(nameof(GetMessages), new { id }, message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You are not a member of this conversation");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }
    }
}