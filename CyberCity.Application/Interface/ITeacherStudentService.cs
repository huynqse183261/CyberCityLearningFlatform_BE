using CyberCity.DTOs.TeacherStudents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface ITeacherStudentService
    {
        Task<List<StudentOfTeacherDto>> GetStudentsByTeacherIdAsync(string teacherId);
        Task<List<TeacherOfStudentDto>> GetTeachersByStudentIdAsync(string studentId);
        Task<TeacherStudentDto> AssignTeacherToStudentAsync(AssignTeacherStudentDto assignDto);
        Task<bool> UnassignTeacherStudentAsync(string relationshipId);
        Task<List<TeacherStudentDto>> GetAllRelationshipsAsync();
        Task<bool> RelationshipExistsAsync(string teacherId, string studentId, string courseId);
    }
}