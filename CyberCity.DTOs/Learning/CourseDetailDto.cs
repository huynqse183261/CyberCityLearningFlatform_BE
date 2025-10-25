using CyberCity.DTOs.Courses;
using CyberCity.DTOs.Modules;
using CyberCity.DTOs.Enrollments;

namespace CyberCity.DTOs.Learning
{
    public class CourseDetailDto
    {
        public CourseDto Course { get; set; }
        public List<ModuleDto> Modules { get; set; }
        public CourseEnrollmentDto Enrollment { get; set; }
        public CourseProgressSummaryDto Progress { get; set; }
    }

    public class CourseProgressSummaryDto
    {
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public decimal Percentage { get; set; }
    }
}

