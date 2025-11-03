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

        public async Task<MessagesListResponse> GetMessagesAsync(string conversationId, GetMessagesQuery query)
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

        public async Task<SendMessageResponse> SendMessageAsync(string conversationId, string adminUserId, SendMessageRequest request)
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

        public async Task<DeleteMessageResponse> DeleteMessageAsync(string messageId)
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

        /// <summary>
        /// Kiểm tra user có thể liên hệ admin không (đã mua gói)
        /// </summary>
        public async Task<bool> CanUserContactAdminAsync(string userUid)
        {
            try
            {
                return await _adminMessageRepository.HasUserPurchasedPlanAsync(userUid);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking user purchase status: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// User gửi tin nhắn cho admin (chỉ cho phép user đã mua gói)
        /// </summary>
        public async Task<SendMessageResponse> SendMessageToAdminAsync(string userUid, SendMessageRequest request)
        {
            try
            {
                // Kiểm tra user đã mua gói chưa
                var hasPurchased = await _adminMessageRepository.HasUserPurchasedPlanAsync(userUid);
                if (!hasPurchased)
                {
                    return new SendMessageResponse
                    {
                        IsSuccess = false,
                        Message = "Only users who have purchased a plan can contact admin support."
                    };
                }

                // Lấy hoặc tạo conversation
                var conversationId = await _adminMessageRepository.GetOrCreateUserAdminConversationAsync(userUid);

                // Gửi tin nhắn
                var messageDto = await _adminMessageRepository.SendUserMessageToAdminAsync(conversationId, userUid, request.Message);

                if (messageDto == null)
                {
                    return new SendMessageResponse
                    {
                        IsSuccess = false,
                        Message = "Failed to send message. User not found."
                    };
                }

                return new SendMessageResponse
                {
                    IsSuccess = true,
                    Message = "Message sent to admin successfully",
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

        /// <summary>
        /// Lấy tin nhắn trong cuộc hội thoại của user với admin
        /// </summary>
        public async Task<MessagesListResponse> GetUserConversationWithAdminAsync(string userUid, GetMessagesQuery query)
        {
            try
            {
                // Kiểm tra user đã mua gói chưa
                var hasPurchased = await _adminMessageRepository.HasUserPurchasedPlanAsync(userUid);
                if (!hasPurchased)
                {
                    return new MessagesListResponse
                    {
                        Items = new List<AdminMessageDto>(),
                        TotalItems = 0,
                        PageNumber = query.PageNumber,
                        PageSize = query.PageSize,
                        TotalPages = 0
                    };
                }

                // Lấy hoặc tạo conversation
                var conversationId = await _adminMessageRepository.GetOrCreateUserAdminConversationAsync(userUid);

                // Lấy tin nhắn
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
                throw new Exception($"Error getting user messages: {ex.Message}", ex);
            }
        }
    }
}
