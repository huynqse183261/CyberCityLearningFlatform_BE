using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Topics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.AutoMapper
{
    public class TopicProfile : Profile
    {
        public TopicProfile()
        {
            CreateMap<Topic,TopicDetailDto>().ReverseMap();
            CreateMap<TopicCreateDto,Topic>();
            CreateMap<TopicUpdateDto,Topic>();
        }
    }
}
