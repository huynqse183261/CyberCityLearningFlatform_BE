using CyberCity.DTOs.Admin;

namespace CyberCity.Application.Interface
{
    public interface IAdminMessageService
    {
        Task<ConversationsListResponse> GetConversationsAsync(GetConversationsQuery query);
        Task<MessagesListResponse> GetMessagesAsync(string conversationId, GetMessagesQuery query);
        Task<SendMessageResponse> SendMessageAsync(string conversationId, string adminUserId, SendMessageRequest request);
        Task<DeleteMessageResponse> DeleteMessageAsync(string messageId);
        Task<MessageStatsResponse> GetStatsAsync();
        
        // User endpoints
        Task<SendMessageResponse> SendMessageToAdminAsync(string userUid, SendMessageRequest request);
        Task<MessagesListResponse> GetUserConversationWithAdminAsync(string userUid, GetMessagesQuery query);
        Task<bool> CanUserContactAdminAsync(string userUid);
    }
}
