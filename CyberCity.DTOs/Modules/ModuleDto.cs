namespace CyberCity.DTOs.Modules
{
    public class ModuleDto
    {
        public string Uid { get; set; }
        public string CourseUid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int OrderIndex { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

