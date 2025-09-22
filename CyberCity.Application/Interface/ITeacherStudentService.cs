using CyberCity.DTOs.TeacherStudents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface ITeacherStudentService
    {
        Task<List<StudentOfTeacherDto>> GetStudentsByTeacherIdAsync(Guid teacherId);
        Task<List<TeacherOfStudentDto>> GetTeachersByStudentIdAsync(Guid studentId);
        Task<TeacherStudentDto> AssignTeacherToStudentAsync(AssignTeacherStudentDto assignDto);
        Task<bool> UnassignTeacherStudentAsync(Guid relationshipId);
        Task<List<TeacherStudentDto>> GetAllRelationshipsAsync();
        Task<bool> RelationshipExistsAsync(Guid teacherId, Guid studentId, Guid courseId);
    }
}