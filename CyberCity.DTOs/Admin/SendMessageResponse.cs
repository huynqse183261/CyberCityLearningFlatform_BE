namespace CyberCity.DTOs.Admin
{
    public class SendMessageResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public AdminMessageDto? Data { get; set; }
    }
}
