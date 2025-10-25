using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Answers;

namespace CyberCity.AutoMapper
{
    public class AnswerProfile : Profile
    {
        public AnswerProfile()
        {
            CreateMap<Answer, AnswerDto>();
            CreateMap<AnswerDto, Answer>();

            CreateMap<SubtopicProgress, SubtopicProgressDto>();
            CreateMap<SubtopicProgressDto, SubtopicProgress>();
        }
    }
}

