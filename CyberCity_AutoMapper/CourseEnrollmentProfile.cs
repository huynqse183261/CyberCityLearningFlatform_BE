using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Enrollments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.AutoMapper
{
    public class CourseEnrollmentProfile: Profile
    {
        public CourseEnrollmentProfile()
        {
            CreateMap<CourseEnrollment, EnrollmentDto>()
             .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.CourseU.Title));

            CreateMap<CourseEnrollment, CourseEnrollmentDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.UserU.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserU.Email));
        }
    }
}
