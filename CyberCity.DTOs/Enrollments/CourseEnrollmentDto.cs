using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.DTOs.Enrollments
{
    public class CourseEnrollmentDto
    {
        public Guid UserUid { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
