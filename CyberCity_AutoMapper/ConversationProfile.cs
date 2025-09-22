using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Conversations;
using CyberCity.DTOs.Messages;
using System;
using System.Linq;

namespace CyberCity_AutoMapper
{
    public class ConversationProfile : Profile
    {
        public ConversationProfile()
        {
            CreateMap<Conversation, ConversationDto>()
                .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.ConversationMembers))
                .ForMember(dest => dest.MessageCount, opt => opt.MapFrom(src => src.Messages.Count))
                .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src => 
                    src.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()));

            CreateMap<ConversationMember, ConversationMemberDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserU.Username))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.UserU.FullName))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.UserU.Image));

            CreateMap<CreateConversationDto, Conversation>()
                .ForMember(dest => dest.Uid, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now));
        }
    }
}