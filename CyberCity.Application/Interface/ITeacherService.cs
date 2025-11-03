using CyberCity.DTOs.Teacher;

namespace CyberCity.Application.Interface
{
    public interface ITeacherService
    {
        // Student Management
        Task<TeacherStudentListResponse> GetStudentsAsync(string teacherUid, int page, int limit, string? search, string? courseUid);
        Task<TeacherStudentDetailResponse> GetStudentDetailAsync(string teacherUid, string studentUid);
        Task<AddStudentResponse> AddStudentAsync(string teacherUid, AddStudentRequest request);
        Task<RemoveStudentResponse> RemoveStudentAsync(string teacherUid, string studentUid, string courseUid);

        // Progress
        Task<StudentProgressResponse> GetStudentProgressAsync(string teacherUid, string studentUid, string? courseUid);

        // Conversations
        Task<ConversationsListResponse> GetConversationsAsync(string teacherUid, int page, int limit);
        Task<CreateConversationResponse> CreateConversationAsync(string teacherUid, CreateConversationRequest request);
        Task<ConversationMessagesResponse> GetConversationMessagesAsync(string teacherUid, string conversationUid, int page, int limit, DateTime? before);
        Task<SendMessageResponse> SendMessageAsync(string teacherUid, string conversationUid, SendMessageRequest request);
        Task<MarkAsReadResponse> MarkAsReadAsync(string teacherUid, string conversationUid);

        // Dashboard
        Task<TeacherDashboardStatsResponse> GetDashboardStatsAsync(string teacherUid);
    }
}
