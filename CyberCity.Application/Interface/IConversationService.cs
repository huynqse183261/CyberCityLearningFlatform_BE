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
        Task<List<ConversationDto>> GetConversationsByUserIdAsync(Guid userId);
        Task<PagedResult<ConversationDto>> GetUserConversationsAsync(Guid userId, int pageNumber = 1, int pageSize = 20);
        Task<ConversationDto> GetConversationByIdAsync(Guid conversationId, Guid requestingUserId);
        Task<ConversationDto> CreateConversationAsync(CreateConversationDto createDto, Guid creatorId);
        Task<bool> UpdateConversationMembersAsync(Guid conversationId, UpdateConversationMembersDto updateDto, Guid requestingUserId);
        Task<bool> IsUserMemberOfConversationAsync(Guid conversationId, Guid userId);
    }
}