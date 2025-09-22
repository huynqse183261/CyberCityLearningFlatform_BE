using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Lessons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.AutoMapper
{
    public class LessonProfile : Profile
    {
        public LessonProfile()
        {
            CreateMap<Lesson, LessonDetailResponse>().ReverseMap();
            CreateMap<LessonCreateRequest, Lesson>();
            CreateMap<LessonUpdateDto, Lesson>();
        }
    }
}
