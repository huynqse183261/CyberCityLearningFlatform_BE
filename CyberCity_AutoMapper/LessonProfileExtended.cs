using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Lessons;

namespace CyberCity.AutoMapper
{
    public class LessonProfileExtended : Profile
    {
        public LessonProfileExtended()
        {
            CreateMap<Lesson, LessonDto>();
            CreateMap<LessonDto, Lesson>();
        }
    }
}

