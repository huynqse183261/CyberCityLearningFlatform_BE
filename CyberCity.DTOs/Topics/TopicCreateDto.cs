using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Topics
{
    public class TopicCreateDto
    {

        public string LessonUid { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? OrderIndex { get; set; }
    }
}
