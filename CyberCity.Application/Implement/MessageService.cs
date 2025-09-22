using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.DBcontext;
using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Messages;
using CyberCity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Application.Implement
{
    public class MessageService : IMessageService
    {
        private readonly MessageRepo _messageRepo;
        private readonly ConversationRepo _conversationRepo;
        private readonly CyberCityLearningFlatFormDBContext _context;
        private readonly IMapper _mapper;

        public MessageService(
            MessageRepo messageRepo,
            ConversationRepo conversationRepo,
            CyberCityLearningFlatFormDBContext context,
            IMapper mapper)
        {
            _messageRepo = messageRepo;
            _conversationRepo = conversationRepo;
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<MessageDto>> GetMessagesByConversationIdAsync(Guid conversationId, Guid requestingUserId, int pageNumber = 1, int pageSize = 50)
        {
            // Check if user is member of conversation
            var isMember = await _conversationRepo.IsUserMemberOfConversationAsync(conversationId, requestingUserId);
            if (!isMember)
                throw new UnauthorizedAccessException("User is not a member of this conversation");

            var messages = await _messageRepo.GetMessagesByConversationIdAsync(conversationId, pageNumber, pageSize);
            var totalMessages = await _messageRepo.GetMessageCountByConversationIdAsync(conversationId);

            var totalPages = (int)Math.Ceiling(totalMessages / (double)pageSize);

            return new PagedResult<MessageDto>
            {
                Items = messages.Select(m => _mapper.Map<MessageDto>(m)).ToList(),
                TotalItems = totalMessages,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<MessageDto> SendMessageAsync(Guid conversationId, CreateMessageDto createDto, Guid senderId)
        {
            // Check if sender is member of conversation
            var isMember = await _conversationRepo.IsUserMemberOfConversationAsync(conversationId, senderId);
            if (!isMember)
                throw new UnauthorizedAccessException("User is not a member of this conversation");

            var message = new Message
            {
                Uid = Guid.NewGuid(),
                ConversationUid = conversationId,
                SenderUid = senderId,
                Message1 = createDto.Message,
                SentAt = DateTime.Now
            };

            await _messageRepo.CreateAsync(message);

            // Get the created message with sender info
            var createdMessage = await _context.Messages
                .Include(m => m.SenderU)
                .FirstOrDefaultAsync(m => m.Uid == message.Uid);

            return _mapper.Map<MessageDto>(createdMessage);
        }
    }
}