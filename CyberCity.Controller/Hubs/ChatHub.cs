using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using CyberCity.Application.Interface;
using CyberCity.DTOs.Messages;
using System.Collections.Concurrent;

namespace CyberCity.Controller.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IConversationService _conversationService;
        private static readonly ConcurrentDictionary<string, string> ConnectedUsers = new();

        public ChatHub(IMessageService messageService, IConversationService conversationService)
        {
            _messageService = messageService;
            _conversationService = conversationService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetCurrentUserId();
            ConnectedUsers[Context.ConnectionId] = userId;
            
            // Join user to their conversation groups
            var userConversations = await _conversationService.GetUserConversationsAsync(GetCurrentUserId());
            foreach (var conversation in userConversations.Items)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversation.Uid}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ConnectedUsers.TryRemove(Context.ConnectionId, out _);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinConversation(string conversationId)
        {
            if (!string.IsNullOrEmpty(conversationId))
            {
                // Verify user is member of conversation
                var isMember = await _conversationService.IsUserMemberOfConversationAsync(conversationId, GetCurrentUserId());
                if (isMember)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
                    await Clients.Caller.SendAsync("JoinedConversation", conversationId);
                }
                else
                {
                    await Clients.Caller.SendAsync("Error", "You are not a member of this conversation");
                }
            }
        }

        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
            await Clients.Caller.SendAsync("LeftConversation", conversationId);
        }

        public async Task SendMessage(string conversationId, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(conversationId))
                {
                    await Clients.Caller.SendAsync("Error", "Invalid conversation ID");
                    return;
                }

                var createDto = new CreateMessageDto { Message = message };
                var messageDto = await _messageService.SendMessageAsync(conversationId, createDto, GetCurrentUserId());

                // Send to all members of the conversation
                await Clients.Group($"conversation_{conversationId}").SendAsync("ReceiveMessage", messageDto);
            }
            catch (UnauthorizedAccessException)
            {
                await Clients.Caller.SendAsync("Error", "You are not authorized to send messages to this conversation");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        public async Task MarkAsRead(string conversationId)
        {
            // This can be implemented later for read receipts
            await Clients.OthersInGroup($"conversation_{conversationId}")
                .SendAsync("MessageRead", conversationId, GetCurrentUserId());
        }

        public async Task TypingStart(string conversationId)
        {
            var userId = GetCurrentUserId();
            await Clients.OthersInGroup($"conversation_{conversationId}")
                .SendAsync("UserTyping", conversationId, userId, true);
        }

        public async Task TypingStop(string conversationId)
        {
            var userId = GetCurrentUserId();
            await Clients.OthersInGroup($"conversation_{conversationId}")
                .SendAsync("UserTyping", conversationId, userId, false);
        }

        private string GetCurrentUserId()
        {
            // Try to get from "sub" claim first (standard JWT)
            var userIdClaim = Context.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            
            // If not found, try "uid" claim
            if (string.IsNullOrEmpty(userIdClaim))
            {
                userIdClaim = Context.User?.FindFirst("uid")?.Value;
            }
            
            // If still not found, try NameIdentifier
            if (string.IsNullOrEmpty(userIdClaim))
            {
                userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("Invalid user token");
            }
            return userIdClaim;
        }
    }
}