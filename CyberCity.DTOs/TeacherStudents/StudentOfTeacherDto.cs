using System;

namespace CyberCity.DTOs.TeacherStudents
{
    public class StudentOfTeacherDto
    {
        public Guid StudentUid { get; set; }
        public string StudentUsername { get; set; }
        public string StudentFullName { get; set; }
        public string StudentEmail { get; set; }
        public string StudentImage { get; set; }
        public Guid CourseUid { get; set; }
        public string CourseName { get; set; }
        public Guid RelationshipUid { get; set; } // TeacherStudent.Uid for deletion
    }
}