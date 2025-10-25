namespace CyberCity.DTOs.Teacher
{
    public class TeacherDashboardStatsResponse
    {
        public bool Success { get; set; }
        public DashboardStatsDataDto Data { get; set; } = new DashboardStatsDataDto();
    }

    public class DashboardStatsDataDto
    {
        public int TotalStudents { get; set; }
        public int ActiveCourses { get; set; }
        public int UnreadMessages { get; set; }
        public double AvgProgress { get; set; }
    }
}
