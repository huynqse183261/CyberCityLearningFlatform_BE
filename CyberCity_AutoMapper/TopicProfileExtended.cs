using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Topics;

namespace CyberCity.AutoMapper
{
    public class TopicProfileExtended : Profile
    {
        public TopicProfileExtended()
        {
            CreateMap<Topic, TopicDto>();
            CreateMap<TopicDto, Topic>();
        }
    }
}

