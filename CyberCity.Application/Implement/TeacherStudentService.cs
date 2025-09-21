using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs.TeacherStudents;
using CyberCity.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Application.Implement
{
    public class TeacherStudentService : ITeacherStudentService
    {
        private readonly TeacherStudentRepo _teacherStudentRepo;
        private readonly UserRepo _userRepo;
        private readonly CourseRepo _courseRepo;
        private readonly IMapper _mapper;

        public TeacherStudentService(
            TeacherStudentRepo teacherStudentRepo,
            UserRepo userRepo,
            CourseRepo courseRepo,
            IMapper mapper)
        {
            _teacherStudentRepo = teacherStudentRepo;
            _userRepo = userRepo;
            _courseRepo = courseRepo;
            _mapper = mapper;
        }

        public async Task<List<StudentOfTeacherDto>> GetStudentsByTeacherIdAsync(Guid teacherId)
        {
            // Verify teacher exists and has teacher role
            var teacher = await _userRepo.GetByIdAsync(teacherId);
            if (teacher == null || teacher.Role != "teacher")
                throw new ArgumentException("Teacher not found or user is not a teacher");

            var relationships = await _teacherStudentRepo.GetStudentsByTeacherIdAsync(teacherId);
            return relationships.Select(r => _mapper.Map<StudentOfTeacherDto>(r)).ToList();
        }

        public async Task<List<TeacherOfStudentDto>> GetTeachersByStudentIdAsync(Guid studentId)
        {
            // Verify student exists and has student role
            var student = await _userRepo.GetByIdAsync(studentId);
            if (student == null || student.Role != "student")
                throw new ArgumentException("Student not found or user is not a student");

            var relationships = await _teacherStudentRepo.GetTeachersByStudentIdAsync(studentId);
            return relationships.Select(r => _mapper.Map<TeacherOfStudentDto>(r)).ToList();
        }

        public async Task<TeacherStudentDto> AssignTeacherToStudentAsync(AssignTeacherStudentDto assignDto)
        {
            // Verify teacher exists and has teacher role
            var teacher = await _userRepo.GetByIdAsync(assignDto.TeacherUid);
            if (teacher == null || teacher.Role != "teacher")
                throw new ArgumentException("Teacher not found or user is not a teacher");

            // Verify student exists and has student role
            var student = await _userRepo.GetByIdAsync(assignDto.StudentUid);
            if (student == null || student.Role != "student")
                throw new ArgumentException("Student not found or user is not a student");

            // Verify course exists
            var course = await _courseRepo.GetByIdAsync(assignDto.CourseUid);
            if (course == null)
                throw new ArgumentException("Course not found");

            // Check if relationship already exists
            var existingRelationship = await _teacherStudentRepo.RelationshipExistsAsync(
                assignDto.TeacherUid, assignDto.StudentUid, assignDto.CourseUid);
            if (existingRelationship)
                throw new ArgumentException("Teacher-Student relationship already exists for this course");

            // Create new relationship
            var teacherStudent = new TeacherStudent
            {
                Uid = Guid.NewGuid(),
                TeacherUid = assignDto.TeacherUid,
                StudentUid = assignDto.StudentUid,
                CourseUid = assignDto.CourseUid
            };

            await _teacherStudentRepo.CreateAsync(teacherStudent);

            // Get the created relationship with details
            var createdRelationship = await _teacherStudentRepo.GetByIdWithDetailsAsync(teacherStudent.Uid);
            return _mapper.Map<TeacherStudentDto>(createdRelationship);
        }

        public async Task<bool> UnassignTeacherStudentAsync(Guid relationshipId)
        {
            var relationship = await _teacherStudentRepo.GetByIdAsync(relationshipId);
            if (relationship == null)
                return false;

            await _teacherStudentRepo.RemoveAsync(relationship);
            return true;
        }

        public async Task<List<TeacherStudentDto>> GetAllRelationshipsAsync()
        {
            var relationships = await _teacherStudentRepo.GetAllWithDetailsAsync();
            return relationships.Select(r => _mapper.Map<TeacherStudentDto>(r)).ToList();
        }

        public async Task<bool> RelationshipExistsAsync(Guid teacherId, Guid studentId, Guid courseId)
        {
            return await _teacherStudentRepo.RelationshipExistsAsync(teacherId, studentId, courseId);
        }
    }
}