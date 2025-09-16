using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Lessons
{
    public class LessonCreateRequest
    {

        public Guid ModuleUid { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string LessonType { get; set; }

        public int? OrderIndex { get; set; }

    }
}
