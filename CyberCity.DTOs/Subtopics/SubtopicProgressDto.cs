using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Subtopics
{
    public class SubtopicProgressDto
    {
        public string Uid { get; set; }

        public string StudentUid { get; set; }

        public string SubtopicUid { get; set; }

        public bool? IsCompleted { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}
