namespace CyberCity.DTOs.Teacher
{
    public class TeacherStudentListResponse
    {
        public bool Success { get; set; }
        public List<TeacherStudentListDto> Students { get; set; } = new List<TeacherStudentListDto>();
        public PaginationDto Pagination { get; set; } = new PaginationDto();
    }
}
