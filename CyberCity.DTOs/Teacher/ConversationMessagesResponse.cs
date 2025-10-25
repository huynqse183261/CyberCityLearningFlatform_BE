namespace CyberCity.DTOs.Teacher
{
    public class ConversationMessagesResponse
    {
        public bool Success { get; set; }
        public string ConversationUid { get; set; }
        public ParticipantDto? Participant { get; set; }
        public List<TeacherMessageDto> Messages { get; set; } = new List<TeacherMessageDto>();
        public MessagesPaginationDto Pagination { get; set; } = new MessagesPaginationDto();
    }

    public class ParticipantDto
    {
        public string Uid { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class MessagesPaginationDto
    {
        public int CurrentPage { get; set; }
        public bool HasMore { get; set; }
    }
}
