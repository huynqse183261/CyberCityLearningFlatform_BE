using CyberCity.DTOs.Learning;
using CyberCity.DTOs.Answers;
using CyberCity.DTOs.Courses;

namespace CyberCity.Application.Interface
{
    public interface ILearningService
    {
        // Course methods
        Task<List<CourseDto>> GetAllCoursesAsync(string? studentId = null);
        Task<CourseDetailDto> GetCourseDetailAsync(string courseId, string studentId);
        
        // Module methods
        Task<ModuleDetailResponseDto> GetModuleDetailAsync(string moduleId, string studentId);
        
        // Lesson methods
        Task<LearningContentDto> GetLessonContentAsync(string lessonId, string studentId);
        
        // Subtopic methods
        Task<SubmitAnswerResponseDto> SubmitSubtopicAnswerAsync(string subtopicId, string studentId, SubmitAnswerDto submitDto);
        Task<SubtopicProgressDto> CompleteSubtopicAsync(string subtopicId, string studentId);
        
        // Progress methods
        Task<StudentProgressDto> GetStudentProgressAsync(string studentId);
        Task<CourseProgressDetailDto> GetCourseProgressDetailAsync(string courseId, string studentId);
    }
}

