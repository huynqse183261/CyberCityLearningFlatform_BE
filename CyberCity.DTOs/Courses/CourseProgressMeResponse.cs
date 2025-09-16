using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Courses
{
    public class CourseProgressMeResponse
    {
        public Guid CourseUid { get; set; }
        public decimal ProgressPercent { get; set; }
    }
}
