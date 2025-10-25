namespace CyberCity.DTOs.Labs
{
    public class StartLabResponseDto
    {
        public string SessionId { get; set; }
        public List<LabComponentStatusDto> Components { get; set; }
    }

    public class LabComponentStatusDto
    {
        public string ComponentId { get; set; }
        public string Status { get; set; } // starting, running, stopped
        public string AccessUrl { get; set; }
    }
}

