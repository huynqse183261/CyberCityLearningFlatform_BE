using System;
using System.Collections.Generic;

namespace CyberCity.DTOs.Courses
{
    public class CourseOutlineResponseDto
    {
        public Guid Uid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<ModuleOutlineDto> Modules { get; set; } = new List<ModuleOutlineDto>();
    }

    public class ModuleOutlineDto
    {
        public Guid Uid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? OrderIndex { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<LessonOutlineDto> Lessons { get; set; } = new List<LessonOutlineDto>();
    }

    public class LessonOutlineDto
    {
        public Guid Uid { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string LessonType { get; set; }
        public int? OrderIndex { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<TopicOutlineDto> Topics { get; set; } = new List<TopicOutlineDto>();
    }

    public class TopicOutlineDto
    {
        public Guid Uid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? PageNumber { get; set; }
        public int? OrderIndex { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<SubtopicOutlineDto> Subtopics { get; set; } = new List<SubtopicOutlineDto>();
    }

    public class SubtopicOutlineDto
    {
        public Guid Uid { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? OrderIndex { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}



