using CyberCity.DTOs.Modules;
using CyberCity.DTOs.Lessons;
using CyberCity.DTOs.Labs;

namespace CyberCity.DTOs.Learning
{
    public class ModuleDetailResponseDto
    {
        public ModuleDto Module { get; set; }
        public List<LessonDto> Lessons { get; set; }
        public List<LabDto> Labs { get; set; }
        public ModuleProgressDto Progress { get; set; }
    }

    public class ModuleProgressDto
    {
        public int TotalItems { get; set; }
        public int CompletedItems { get; set; }
        public decimal Percentage { get; set; }
    }
}

