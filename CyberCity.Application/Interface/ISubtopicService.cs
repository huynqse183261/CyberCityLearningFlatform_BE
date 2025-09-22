using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Subtopics;
using CyberCity.DTOs.Topics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface ISubtopicService
    {
        Task<PagedResult<SubtopicDetailDto>> GetSubtopicAsync(int page, int pageSize);
        Task<Subtopic> GetByIdAsync(Guid uid);
        Task<Guid> CreateAsync(Subtopic subtopic);
        Task<bool> UpdateAsync(Subtopic subtopic);
        Task<bool> DeleteAsync(Guid uid);
    }
}
