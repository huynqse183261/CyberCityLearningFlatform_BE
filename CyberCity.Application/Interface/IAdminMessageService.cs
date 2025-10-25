using CyberCity.DTOs.Admin;

namespace CyberCity.Application.Interface
{
    public interface IAdminMessageService
    {
        Task<ConversationsListResponse> GetConversationsAsync(GetConversationsQuery query);
        Task<MessagesListResponse> GetMessagesAsync(Guid conversationId, GetMessagesQuery query);
        Task<SendMessageResponse> SendMessageAsync(Guid conversationId, Guid adminUserId, SendMessageRequest request);
        Task<DeleteMessageResponse> DeleteMessageAsync(Guid messageId);
        Task<MessageStatsResponse> GetStatsAsync();
    }
}
