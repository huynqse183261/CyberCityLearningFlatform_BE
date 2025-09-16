using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Lessons
{
    public class LessonDetailResponse
    {
        public Guid Uid { get; set; }

        public Guid ModuleUid { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string LessonType { get; set; }

        public int? OrderIndex { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
