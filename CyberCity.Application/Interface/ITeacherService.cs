using CyberCity.DTOs.Teacher;

namespace CyberCity.Application.Interface
{
    public interface ITeacherService
    {
        // Student Management
        Task<TeacherStudentListResponse> GetStudentsAsync(Guid teacherUid, int page, int limit, string? search, Guid? courseUid);
        Task<TeacherStudentDetailResponse> GetStudentDetailAsync(Guid teacherUid, Guid studentUid);
        Task<AddStudentResponse> AddStudentAsync(Guid teacherUid, AddStudentRequest request);
        Task<RemoveStudentResponse> RemoveStudentAsync(Guid teacherUid, Guid studentUid, Guid courseUid);

        // Progress
        Task<StudentProgressResponse> GetStudentProgressAsync(Guid teacherUid, Guid studentUid, Guid? courseUid);

        // Conversations
        Task<ConversationsListResponse> GetConversationsAsync(Guid teacherUid, int page, int limit);
        Task<CreateConversationResponse> CreateConversationAsync(Guid teacherUid, CreateConversationRequest request);
        Task<ConversationMessagesResponse> GetConversationMessagesAsync(Guid teacherUid, Guid conversationUid, int page, int limit, DateTime? before);
        Task<SendMessageResponse> SendMessageAsync(Guid teacherUid, Guid conversationUid, SendMessageRequest request);
        Task<MarkAsReadResponse> MarkAsReadAsync(Guid teacherUid, Guid conversationUid);

        // Dashboard
        Task<TeacherDashboardStatsResponse> GetDashboardStatsAsync(Guid teacherUid);
    }
}
