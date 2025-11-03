using CyberCity.DTOs;
using CyberCity.DTOs.Subtopics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface ISubtopicProgressService
    {
        Task<List<SubtopicProgressDto>> GetByStudentAsync(string studentId);
        Task<List<SubtopicProgressDto>> GetBySubtopicAndStudentAsync(string courseId, string studentId);
        Task<SubtopicProgressDto> MarkCompleteAsync(string subtopicId, string studentId);
        Task<PagedResult<SubtopicProgressDto>> GetAllsubtopicProgressAsync(int page ,int number);
    }
}
