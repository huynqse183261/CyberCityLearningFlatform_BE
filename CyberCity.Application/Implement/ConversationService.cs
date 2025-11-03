using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Conversations;
using CyberCity.DTOs.Messages;
using CyberCity.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Application.Implement
{
    public class ConversationService : IConversationService
    {
        private readonly ConversationRepo _conversationRepo;
        private readonly ConversationMemberRepo _conversationMemberRepo;
        private readonly MessageRepo _messageRepo;
        private readonly IMapper _mapper;

        public ConversationService(
            ConversationRepo conversationRepo,
            ConversationMemberRepo conversationMemberRepo,
            MessageRepo messageRepo,
            IMapper mapper)
        {
            _conversationRepo = conversationRepo;
            _conversationMemberRepo = conversationMemberRepo;
            _messageRepo = messageRepo;
            _mapper = mapper;
        }

        public async Task<List<ConversationDto>> GetConversationsByUserIdAsync(string userId)
        {
            var conversations = await _conversationRepo.GetConversationsByUserIdAsync(userId);
            return conversations.Select(c => _mapper.Map<ConversationDto>(c)).ToList();
        }

        public async Task<PagedResult<ConversationDto>> GetUserConversationsAsync(string userId, int pageNumber = 1, int pageSize = 20)
        {
            var conversations = await _conversationRepo.GetConversationsByUserIdAsync(userId);
            var totalConversations = conversations.Count;
            var paginatedConversations = conversations
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalPages = (int)Math.Ceiling(totalConversations / (double)pageSize);

            return new PagedResult<ConversationDto>
            {
                Items = paginatedConversations.Select(c => _mapper.Map<ConversationDto>(c)).ToList(),
                TotalItems = totalConversations,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<ConversationDto> GetConversationByIdAsync(string conversationId, string requestingUserId)
        {
            // Check if user is member of conversation
            var isMember = await _conversationRepo.IsUserMemberOfConversationAsync(conversationId, requestingUserId);
            if (!isMember)
                throw new UnauthorizedAccessException("User is not a member of this conversation");

            var conversation = await _conversationRepo.GetConversationByIdAsync(conversationId);
            if (conversation == null)
                throw new ArgumentException("Conversation not found");

            return _mapper.Map<ConversationDto>(conversation);
        }

        public async Task<ConversationDto> CreateConversationAsync(CreateConversationDto createDto, string creatorId)
        {
            // Create conversation
            var conversation = new Conversation
            {
                Uid = Guid.NewGuid().ToString(),
                Title = createDto.Title,
                IsGroup = createDto.IsGroup,
                OrgUid = createDto.OrgUid,
                CreatedAt = DateTime.Now
            };

            await _conversationRepo.CreateAsync(conversation);

            // Add creator as member
            var allMemberIds = createDto.InitialMemberIds.ToList();
            if (!allMemberIds.Contains(creatorId))
            {
                allMemberIds.Add(creatorId);
            }

            // Add members
            await _conversationMemberRepo.AddMembersAsync(conversation.Uid, allMemberIds.ToArray());

            // Get the created conversation with members
            var createdConversation = await _conversationRepo.GetConversationWithMembersAsync(conversation.Uid);
            return _mapper.Map<ConversationDto>(createdConversation);
        }

        public async Task<bool> UpdateConversationMembersAsync(string conversationId, UpdateConversationMembersDto updateDto, string requestingUserId)
        {
            // Check if user is member of conversation
            var isMember = await _conversationRepo.IsUserMemberOfConversationAsync(conversationId, requestingUserId);
            if (!isMember)
                return false;

            // Add new members
            if (updateDto.MemberIdsToAdd?.Length > 0)
            {
                await _conversationMemberRepo.AddMembersAsync(conversationId, updateDto.MemberIdsToAdd);
            }

            // Remove members
            if (updateDto.MemberIdsToRemove?.Length > 0)
            {
                foreach (var memberIdToRemove in updateDto.MemberIdsToRemove)
                {
                    await _conversationMemberRepo.RemoveMemberAsync(conversationId, memberIdToRemove);
                }
            }

            return true;
        }

        public async Task<bool> IsUserMemberOfConversationAsync(string conversationId, string userId)
        {
            return await _conversationRepo.IsUserMemberOfConversationAsync(conversationId, userId);
        }
    }
}