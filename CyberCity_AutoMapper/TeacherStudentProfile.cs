using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.TeacherStudents;
using System;

namespace CyberCity_AutoMapper
{
    public class TeacherStudentProfile : Profile
    {
        public TeacherStudentProfile()
        {
            CreateMap<TeacherStudent, TeacherStudentDto>()
                .ForMember(dest => dest.TeacherUsername, opt => opt.MapFrom(src => src.TeacherU.Username))
                .ForMember(dest => dest.TeacherFullName, opt => opt.MapFrom(src => src.TeacherU.FullName))
                .ForMember(dest => dest.TeacherEmail, opt => opt.MapFrom(src => src.TeacherU.Email))
                .ForMember(dest => dest.TeacherImage, opt => opt.MapFrom(src => src.TeacherU.Image))
                .ForMember(dest => dest.StudentUsername, opt => opt.MapFrom(src => src.StudentU.Username))
                .ForMember(dest => dest.StudentFullName, opt => opt.MapFrom(src => src.StudentU.FullName))
                .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.StudentU.Email))
                .ForMember(dest => dest.StudentImage, opt => opt.MapFrom(src => src.StudentU.Image))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseU.Title))
                .ForMember(dest => dest.CourseDescription, opt => opt.MapFrom(src => src.CourseU.Description));

            CreateMap<TeacherStudent, StudentOfTeacherDto>()
                .ForMember(dest => dest.StudentUid, opt => opt.MapFrom(src => src.StudentUid))
                .ForMember(dest => dest.StudentUsername, opt => opt.MapFrom(src => src.StudentU.Username))
                .ForMember(dest => dest.StudentFullName, opt => opt.MapFrom(src => src.StudentU.FullName))
                .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.StudentU.Email))
                .ForMember(dest => dest.StudentImage, opt => opt.MapFrom(src => src.StudentU.Image))
                .ForMember(dest => dest.CourseUid, opt => opt.MapFrom(src => src.CourseUid))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseU.Title))
                .ForMember(dest => dest.RelationshipUid, opt => opt.MapFrom(src => src.Uid));

            CreateMap<TeacherStudent, TeacherOfStudentDto>()
                .ForMember(dest => dest.TeacherUid, opt => opt.MapFrom(src => src.TeacherUid))
                .ForMember(dest => dest.TeacherUsername, opt => opt.MapFrom(src => src.TeacherU.Username))
                .ForMember(dest => dest.TeacherFullName, opt => opt.MapFrom(src => src.TeacherU.FullName))
                .ForMember(dest => dest.TeacherEmail, opt => opt.MapFrom(src => src.TeacherU.Email))
                .ForMember(dest => dest.TeacherImage, opt => opt.MapFrom(src => src.TeacherU.Image))
                .ForMember(dest => dest.CourseUid, opt => opt.MapFrom(src => src.CourseUid))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CourseU.Title))
                .ForMember(dest => dest.RelationshipUid, opt => opt.MapFrom(src => src.Uid));

            CreateMap<AssignTeacherStudentDto, TeacherStudent>()
                .ForMember(dest => dest.Uid, opt => opt.MapFrom(_ => Guid.NewGuid().ToString()));
        }
    }
}