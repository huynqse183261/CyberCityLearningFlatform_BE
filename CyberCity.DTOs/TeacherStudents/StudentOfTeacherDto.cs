using System;

namespace CyberCity.DTOs.TeacherStudents
{
    public class StudentOfTeacherDto
    {
        public string StudentUid { get; set; }
        public string StudentUsername { get; set; }
        public string StudentFullName { get; set; }
        public string StudentEmail { get; set; }
        public string StudentImage { get; set; }
        public string CourseUid { get; set; }
        public string CourseName { get; set; }
        public string RelationshipUid { get; set; } // TeacherStudent.Uid for deletion
    }
}