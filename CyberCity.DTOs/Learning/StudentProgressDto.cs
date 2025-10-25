namespace CyberCity.DTOs.Learning
{
    public class StudentProgressDto
    {
        public int TotalCourses { get; set; }
        public int EnrolledCourses { get; set; }
        public int CompletedCourses { get; set; }
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public int TotalQuizzes { get; set; }
        public int CompletedQuizzes { get; set; }
        public int TotalLabs { get; set; }
        public int CompletedLabs { get; set; }
        public decimal OverallProgress { get; set; }
    }

    public class CourseProgressDetailDto
    {
        public CourseProgressInfoDto Course { get; set; }
        public List<ModuleProgressDetailDto> Modules { get; set; }
        public decimal CourseProgress { get; set; }
    }

    public class CourseProgressInfoDto
    {
        public string Uid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
    }

    public class ModuleProgressDetailDto
    {
        public ModuleProgressInfoDto Module { get; set; }
        public List<LessonProgressDto> LessonsProgress { get; set; }
        public List<LabProgressDto> LabsProgress { get; set; }
        public decimal ModuleProgress { get; set; }
    }

    public class ModuleProgressInfoDto
    {
        public string Uid { get; set; }
        public string Title { get; set; }
        public int OrderIndex { get; set; }
    }

    public class LessonProgressDto
    {
        public LessonProgressInfoDto Lesson { get; set; }
        public int SubtopicsCompleted { get; set; }
        public int SubtopicsTotal { get; set; }
        public decimal? QuizScore { get; set; }
    }

    public class LessonProgressInfoDto
    {
        public string Uid { get; set; }
        public string Title { get; set; }
        public int OrderIndex { get; set; }
    }

    public class LabProgressDto
    {
        public LabProgressInfoDto Lab { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class LabProgressInfoDto
    {
        public string Uid { get; set; }
        public string Title { get; set; }
        public bool IsRequired { get; set; }
    }
}

