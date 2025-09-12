using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Subtopics
{
    public class SubtopicProgressDto
    {
        public Guid Uid { get; set; }

        public Guid StudentUid { get; set; }

        public Guid SubtopicUid { get; set; }

        public bool? IsCompleted { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}
