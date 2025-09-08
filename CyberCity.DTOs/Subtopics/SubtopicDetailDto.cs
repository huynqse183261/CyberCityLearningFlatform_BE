using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Subtopics
{
    public class SubtopicDetailDto
    {
        public Guid Uid { get; set; }

        public Guid TopicUid { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int? OrderIndex { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
