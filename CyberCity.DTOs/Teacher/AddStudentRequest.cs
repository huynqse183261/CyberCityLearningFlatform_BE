using System.ComponentModel.DataAnnotations;

namespace CyberCity.DTOs.Teacher
{
    public class AddStudentRequest
    {
        [Required]
        public string StudentUid { get; set; }
        
        [Required]
        public string CourseUid { get; set; }
    }
}
