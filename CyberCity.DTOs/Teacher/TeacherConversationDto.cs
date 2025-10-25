namespace CyberCity.DTOs.Teacher
{
    public class TeacherConversationDto
    {
        public string ConversationUid { get; set; }
        public string ParticipantUid { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }
}
