using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.AutoMapper
{
    public class CourseProgressProfile : Profile
    {
        public CourseProgressProfile()
        {
            // Map to overview item (for admin/teacher view)
            CreateMap<CourseProgress, CourseProgressOverviewItemDto>()
                .ForMember(dest => dest.StudentUid, opt => opt.MapFrom(src => src.StudentUid))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.StudentU != null ? src.StudentU.FullName : string.Empty))
                .ForMember(dest => dest.ProgressPercent, opt => opt.MapFrom(src => src.ProgressPercent ?? 0))
                .ForMember(dest => dest.LastAccessedAt, opt => opt.MapFrom(src => src.LastAccessedAt));

            // Map to "me" progress (for student view)
            CreateMap<CourseProgress, CourseProgressMeDto>()
                .ForMember(dest => dest.CourseUid, opt => opt.MapFrom(src => src.CourseUid))
                .ForMember(dest => dest.ProgressPercent, opt => opt.MapFrom(src => src.ProgressPercent ?? 0));
        }
    }
}
