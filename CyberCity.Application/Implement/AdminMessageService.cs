using CyberCity.Application.Interface;
using CyberCity.DTOs.Admin;
using CyberCity.Infrastructure;

namespace CyberCity.Application.Implement
{
    public class AdminMessageService : IAdminMessageService
    {
        private readonly IAdminMessageRepository _adminMessageRepository;

        public AdminMessageService(IAdminMessageRepository adminMessageRepository)
        {
            _adminMessageRepository = adminMessageRepository;
        }

        public async Task<ConversationsListResponse> GetConversationsAsync(GetConversationsQuery query)
        {
            try
            {
                var (conversations, totalCount) = await _adminMessageRepository.GetConversationsAsync(query);

                return new ConversationsListResponse
                {
                    Items = conversations,
                    TotalItems = totalCount,
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting conversations: {ex.Message}", ex);
            }
        }

        public async Task<MessagesListResponse> GetMessagesAsync(Guid conversationId, GetMessagesQuery query)
        {
            try
            {
                var (messages, totalCount) = await _adminMessageRepository.GetMessagesAsync(conversationId, query);

                return new MessagesListResponse
                {
                    Items = messages,
                    TotalItems = totalCount,
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting messages: {ex.Message}", ex);
            }
        }

        public async Task<SendMessageResponse> SendMessageAsync(Guid conversationId, Guid adminUserId, SendMessageRequest request)
        {
            try
            {
                var messageDto = await _adminMessageRepository.SendMessageAsync(conversationId, adminUserId, request.Message);

                if (messageDto == null)
                {
                    return new SendMessageResponse
                    {
                        IsSuccess = false,
                        Message = "Failed to send message. Admin user not found."
                    };
                }

                return new SendMessageResponse
                {
                    IsSuccess = true,
                    Message = "Message sent successfully",
                    Data = messageDto
                };
            }
            catch (Exception ex)
            {
                return new SendMessageResponse
                {
                    IsSuccess = false,
                    Message = $"Error sending message: {ex.Message}"
                };
            }
        }

        public async Task<DeleteMessageResponse> DeleteMessageAsync(Guid messageId)
        {
            try
            {
                var result = await _adminMessageRepository.DeleteMessageAsync(messageId);

                if (!result)
                {
                    return new DeleteMessageResponse
                    {
                        IsSuccess = false,
                        Message = "Message not found"
                    };
                }

                return new DeleteMessageResponse
                {
                    IsSuccess = true,
                    Message = "Message deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new DeleteMessageResponse
                {
                    IsSuccess = false,
                    Message = $"Error deleting message: {ex.Message}"
                };
            }
        }

        public async Task<MessageStatsResponse> GetStatsAsync()
        {
            try
            {
                return await _adminMessageRepository.GetStatsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting stats: {ex.Message}", ex);
            }
        }
    }
}
