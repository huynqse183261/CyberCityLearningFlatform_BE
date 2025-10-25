using System;

namespace CyberCity.DTOs.TeacherStudents
{
    public class TeacherStudentDto
    {
        public string Uid { get; set; }
        public string TeacherUid { get; set; }
        public string StudentUid { get; set; }
        public string CourseUid { get; set; }
        
        // Teacher info
        public string TeacherUsername { get; set; }
        public string TeacherFullName { get; set; }
        public string TeacherEmail { get; set; }
        public string TeacherImage { get; set; }
        
        // Student info
        public string StudentUsername { get; set; }
        public string StudentFullName { get; set; }
        public string StudentEmail { get; set; }
        public string StudentImage { get; set; }
        
        // Course info
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }
    }
}