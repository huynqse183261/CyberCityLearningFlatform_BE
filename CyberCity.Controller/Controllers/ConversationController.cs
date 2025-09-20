using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.DTOs.Conversations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CyberCity.Controller.Controllers
{
    [Route("api/conversations")]
    [ApiController]
    [Authorize]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        private readonly IMapper _mapper;

        public ConversationController(IConversationService conversationService, IMapper mapper)
        {
            _conversationService = conversationService;
            _mapper = mapper;
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
                throw new UnauthorizedAccessException("Invalid user token");
            }
            return userId;
        }

        /// <summary>
        /// GET /api/conversations/user/{userId} - Cuộc trò chuyện của user
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<ConversationDto>>> GetConversationsByUserId(Guid userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                
                // Only allow users to get their own conversations unless they are admin
                if (currentUserId != userId)
                {
                    // You can add admin role check here if needed
                    return Forbid("You can only access your own conversations");
                }

                var conversations = await _conversationService.GetConversationsByUserIdAsync(userId);
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/conversations - Tạo cuộc trò chuyện
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ConversationDto>> CreateConversation([FromBody] CreateConversationDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var currentUserId = GetCurrentUserId();
                var conversation = await _conversationService.CreateConversationAsync(createDto, currentUserId);
                return CreatedAtAction(nameof(GetConversationById), new { id = conversation.Uid }, conversation);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// GET /api/conversations/{id} - Lấy thông tin cuộc trò chuyện
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ConversationDto>> GetConversationById(Guid id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var conversation = await _conversationService.GetConversationByIdAsync(id, currentUserId);
                return Ok(conversation);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You are not a member of this conversation");
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// PUT /api/conversations/{id}/members - Thêm/xóa thành viên
        /// </summary>
        [HttpPut("{id}/members")]
        public async Task<ActionResult> UpdateConversationMembers(Guid id, [FromBody] UpdateConversationMembersDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var currentUserId = GetCurrentUserId();
                var success = await _conversationService.UpdateConversationMembersAsync(id, updateDto, currentUserId);
                
                if (!success)
                    return Forbid("You are not authorized to modify this conversation");

                return Ok(new { message = "Conversation members updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}