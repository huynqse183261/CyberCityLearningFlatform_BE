using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Enrollments;

namespace CyberCity.AutoMapper
{
    public class EnrollmentProfile : Profile
    {
        public EnrollmentProfile()
        {
            CreateMap<CourseEnrollment, CourseEnrollmentDto>();
            CreateMap<CourseEnrollmentDto, CourseEnrollment>();
        }
    }
}

