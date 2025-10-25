using System;
using System.ComponentModel.DataAnnotations;

namespace CyberCity.DTOs.TeacherStudents
{
    public class AssignTeacherStudentDto
    {
        [Required]
        public string TeacherUid { get; set; }

        [Required]
        public string StudentUid { get; set; }

        [Required]
        public string CourseUid { get; set; }
    }
}