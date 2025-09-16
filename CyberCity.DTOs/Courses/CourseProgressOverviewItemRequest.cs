using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Courses
{
    public class CourseProgressOverviewItemRequest
    {
        public Guid StudentUid { get; set; }
        public string FullName { get; set; }
        public decimal ProgressPercent { get; set; }
        public DateTime? LastAccessedAt { get; set; }
    }
}
