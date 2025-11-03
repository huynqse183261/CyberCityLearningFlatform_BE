using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Conversations;
using CyberCity.DTOs.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface IConversationService
    {
        Task<List<ConversationDto>> GetConversationsByUserIdAsync(string userId);
        Task<PagedResult<ConversationDto>> GetUserConversationsAsync(string userId, int pageNumber = 1, int pageSize = 20);
        Task<ConversationDto> GetConversationByIdAsync(string conversationId, string requestingUserId);
        Task<ConversationDto> CreateConversationAsync(CreateConversationDto createDto, string creatorId);
        Task<bool> UpdateConversationMembersAsync(string conversationId, UpdateConversationMembersDto updateDto, string requestingUserId);
        Task<bool> IsUserMemberOfConversationAsync(string conversationId, string userId);
    }
}