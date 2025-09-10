using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Lessons
{
    public class LessonProgressDto
    {
         public Guid Uid { get; set; }
        public Guid StudentUid { get; set; }
        public Guid LessonUid { get; set; }
        public string Status { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
