namespace CyberCity.DTOs.Enrollments
{
    public class CourseEnrollmentDto
    {
        public string Uid { get; set; }
        public string UserUid { get; set; }
        public string CourseUid { get; set; }
        public DateTime? EnrolledAt { get; set; }
    }
}

