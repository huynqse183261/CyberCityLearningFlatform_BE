using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Topics
{
    public class TopicDetailDto
    {
        public Guid Uid { get; set; }

        public Guid LessonUid { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? PageNumber { get; set; }

        public int? OrderIndex { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
