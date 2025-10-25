using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Courses;

namespace CyberCity.AutoMapper
{
    public class CourseProfileExtended : Profile
    {
        public CourseProfileExtended()
        {
            CreateMap<Course, CourseDto>();
            CreateMap<CourseDto, Course>();
        }
    }
}

