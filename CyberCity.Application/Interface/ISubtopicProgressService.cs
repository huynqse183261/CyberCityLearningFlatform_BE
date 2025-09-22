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
        Task<List<SubtopicProgressDto>> GetByStudentAsync(Guid studentId);
        Task<List<SubtopicProgressDto>> GetBySubtopicAndStudentAsync(Guid courseId, Guid studentId);
        Task<SubtopicProgressDto> MarkCompleteAsync(Guid subtopicId, Guid studentId);
        Task<PagedResult<SubtopicProgressDto>> GetAllsubtopicProgressAsync(int page ,int number);
    }
}
