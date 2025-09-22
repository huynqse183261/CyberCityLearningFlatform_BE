using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Messages;
using System;

namespace CyberCity_AutoMapper
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message1))
                .ForMember(dest => dest.SenderUsername, opt => opt.MapFrom(src => src.SenderU.Username))
                .ForMember(dest => dest.SenderFullName, opt => opt.MapFrom(src => src.SenderU.FullName))
                .ForMember(dest => dest.SenderImage, opt => opt.MapFrom(src => src.SenderU.Image));

            CreateMap<CreateMessageDto, Message>()
                .ForMember(dest => dest.Uid, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.Message1, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.SentAt, opt => opt.MapFrom(_ => DateTime.Now));
        }
    }
}