using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.DTOs.TeacherStudents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CyberCity.Controller.Controllers
{
    [Route("api/teacher-student")]
    [ApiController]
    [Authorize]
    public class TeacherStudentController : ControllerBase
    {
        private readonly ITeacherStudentService _teacherStudentService;
        private readonly IMapper _mapper;

        public TeacherStudentController(ITeacherStudentService teacherStudentService, IMapper mapper)
        {
            _teacherStudentService = teacherStudentService;
            _mapper = mapper;
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }

        /// <summary>
        /// GET /api/teacher-student/teachers/{teacherId}/students - Học viên của giáo viên
        /// </summary>
        [HttpGet("teachers/{teacherId}/students")]
        public async Task<ActionResult<List<StudentOfTeacherDto>>> GetStudentsByTeacherId(Guid teacherId)
        {
            try
            {
                var currentRole = GetCurrentUserRole();
                
                // Only teachers can see their own students, or admins can see all
                if (currentRole != "admin" && currentRole != "teacher")
                {
                    return Forbid("Only teachers and admins can access this endpoint");
                }

                var students = await _teacherStudentService.GetStudentsByTeacherIdAsync(teacherId);
                return Ok(students);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/teacher-student/students/{studentId}/teachers - Giáo viên của học viên
        /// </summary>
        [HttpGet("students/{studentId}/teachers")]
        public async Task<ActionResult<List<TeacherOfStudentDto>>> GetTeachersByStudentId(Guid studentId)
        {
            try
            {
                var currentRole = GetCurrentUserRole();
                
                // Students can see their own teachers, teachers and admins can see all
                if (currentRole != "admin" && currentRole != "teacher" && currentRole != "student")
                {
                    return Forbid("Access denied");
                }

                var teachers = await _teacherStudentService.GetTeachersByStudentIdAsync(studentId);
                return Ok(teachers);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/teacher-student/assign - Gán giáo viên cho học viên
        /// </summary>
        [HttpPost("assign")]
        [Authorize(Roles = "admin,teacher")]
        public async Task<ActionResult<TeacherStudentDto>> AssignTeacherToStudent([FromBody] AssignTeacherStudentDto assignDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var relationship = await _teacherStudentService.AssignTeacherToStudentAsync(assignDto);
                return CreatedAtAction(nameof(GetAllRelationships), new { }, relationship);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// DELETE /api/teacher-student/{id} - Hủy gán
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,teacher")]
        public async Task<ActionResult> UnassignTeacherStudent(Guid id)
        {
            try
            {
                var success = await _teacherStudentService.UnassignTeacherStudentAsync(id);
                if (!success)
                    return NotFound(new { message = "Teacher-Student relationship not found" });

                return Ok(new { message = "Teacher-Student relationship removed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/teacher-student/relationships - Lấy tất cả mối quan hệ (Admin only)
        /// </summary>
        [HttpGet("relationships")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<TeacherStudentDto>>> GetAllRelationships()
        {
            try
            {
                var relationships = await _teacherStudentService.GetAllRelationshipsAsync();
                return Ok(relationships);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/teacher-student/check - Kiểm tra mối quan hệ có tồn tại
        /// </summary>
        [HttpGet("check")]
        public async Task<ActionResult<bool>> CheckRelationshipExists(
            [FromQuery] Guid teacherId, 
            [FromQuery] Guid studentId, 
            [FromQuery] Guid courseId)
        {
            try
            {
                var exists = await _teacherStudentService.RelationshipExistsAsync(teacherId, studentId, courseId);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}