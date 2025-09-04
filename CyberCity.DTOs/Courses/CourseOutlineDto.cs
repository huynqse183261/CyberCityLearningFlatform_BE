using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Courses
{
    public class CourseOutlineDto
    {
        public Guid CourseUid { get; set; }
        public string Title { get; set; }
        public List<TopicDto> Topics { get; set; } = new List<TopicDto>();
    }

    public class TopicDto
    {
        public Guid Uid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? OrderIndex { get; set; }
        public List<SubtopicDto> Subtopics { get; set; } = new List<SubtopicDto>();
    }

    public class SubtopicDto
    {
        public Guid Uid { get; set; }
        public string Title { get; set; }
        public int? OrderIndex { get; set; }
        public List<LessonDto> Lessons { get; set; } = new List<LessonDto>();
    }

    public class LessonDto
    {
        public Guid Uid { get; set; }
        public string Title { get; set; }
        public int? OrderIndex { get; set; }
        public string LessonType { get; set; }
    }
}
