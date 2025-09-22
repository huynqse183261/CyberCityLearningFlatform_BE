using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.AutoMapper
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationRequest>().ReverseMap();

            CreateMap<Notification, NotificationResponse>()
                .ForMember(dest => dest.SenderUsername, opt => opt.MapFrom(src => src.SenderU != null ? src.SenderU.Username : null))
                .ForMember(dest => dest.ReceiverUsername, opt => opt.MapFrom(src => src.ReceiverU != null ? src.ReceiverU.Username : null));
        }
    }
}
