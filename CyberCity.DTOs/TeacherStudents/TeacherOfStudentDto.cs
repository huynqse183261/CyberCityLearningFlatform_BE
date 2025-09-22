using System;

namespace CyberCity.DTOs.TeacherStudents
{
    public class TeacherOfStudentDto
    {
        public Guid TeacherUid { get; set; }
        public string TeacherUsername { get; set; }
        public string TeacherFullName { get; set; }
        public string TeacherEmail { get; set; }
        public string TeacherImage { get; set; }
        public Guid CourseUid { get; set; }
        public string CourseName { get; set; }
        public Guid RelationshipUid { get; set; } // TeacherStudent.Uid for deletion
    }
}