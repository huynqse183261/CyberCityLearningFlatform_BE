using CyberCity.DTOs.Learning;
using CyberCity.DTOs.Answers;
using CyberCity.DTOs.Courses;

namespace CyberCity.Application.Interface
{
    public interface ILearningService
    {
        // Course methods
        Task<List<CourseDto>> GetAllCoursesAsync(Guid? studentId = null);
        Task<CourseDetailDto> GetCourseDetailAsync(Guid courseId, Guid studentId);
        
        // Module methods
        Task<ModuleDetailResponseDto> GetModuleDetailAsync(Guid moduleId, Guid studentId);
        
        // Lesson methods
        Task<LearningContentDto> GetLessonContentAsync(Guid lessonId, Guid studentId);
        
        // Subtopic methods
        Task<SubmitAnswerResponseDto> SubmitSubtopicAnswerAsync(Guid subtopicId, Guid studentId, SubmitAnswerDto submitDto);
        Task<SubtopicProgressDto> CompleteSubtopicAsync(Guid subtopicId, Guid studentId);
        
        // Progress methods
        Task<StudentProgressDto> GetStudentProgressAsync(Guid studentId);
        Task<CourseProgressDetailDto> GetCourseProgressDetailAsync(Guid courseId, Guid studentId);
    }
}

