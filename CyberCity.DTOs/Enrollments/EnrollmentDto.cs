using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Enrollments
{
    public class EnrollmentDto
    {
        public Guid CourseUid { get; set; }
        public string CourseTitle { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
