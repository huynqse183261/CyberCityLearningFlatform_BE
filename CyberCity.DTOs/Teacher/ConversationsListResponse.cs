namespace CyberCity.DTOs.Teacher
{
    public class ConversationsListResponse
    {
        public bool Success { get; set; }
        public List<TeacherConversationDto> Conversations { get; set; } = new List<TeacherConversationDto>();
        public PaginationDto Pagination { get; set; } = new PaginationDto();
    }
}
