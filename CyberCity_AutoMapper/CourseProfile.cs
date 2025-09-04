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
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseListItemDto>();
            CreateMap<Course, CourseDetailDto>();
            CreateMap<CourseCreateUpdateDto, Course>();
        }
    }
}
