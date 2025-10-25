using System;

namespace CyberCity.DTOs.TeacherStudents
{
    public class TeacherOfStudentDto
    {
        public string TeacherUid { get; set; }
        public string TeacherUsername { get; set; }
        public string TeacherFullName { get; set; }
        public string TeacherEmail { get; set; }
        public string TeacherImage { get; set; }
        public string CourseUid { get; set; }
        public string CourseName { get; set; }
        public string RelationshipUid { get; set; } // TeacherStudent.Uid for deletion
    }
}