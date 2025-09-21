using System;
using System.ComponentModel.DataAnnotations;

namespace CyberCity.DTOs.TeacherStudents
{
    public class AssignTeacherStudentDto
    {
        [Required]
        public Guid TeacherUid { get; set; }

        [Required]
        public Guid StudentUid { get; set; }

        [Required]
        public Guid CourseUid { get; set; }
    }
}