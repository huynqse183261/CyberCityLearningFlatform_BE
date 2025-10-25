namespace CyberCity.DTOs.Teacher
{
    public class CreateConversationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public CreateConversationDataDto? Data { get; set; }
    }

    public class CreateConversationDataDto
    {
        public string ConversationUid { get; set; }
        public string Participant1Uid { get; set; }
        public string Participant2Uid { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
