using CyberCity.DTOs;
using CyberCity.DTOs.Messages;
using System;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface IMessageService
    {
        Task<PagedResult<MessageDto>> GetMessagesByConversationIdAsync(Guid conversationId, Guid requestingUserId, int pageNumber = 1, int pageSize = 50);
        Task<MessageDto> SendMessageAsync(Guid conversationId, CreateMessageDto createDto, Guid senderId);
    }
}