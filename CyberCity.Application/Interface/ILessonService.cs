using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Courses;
using CyberCity.DTOs.Lessons;
using CyberCity.DTOs.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface ILessonService
    {
        Task<PagedResult<LessonDetailDto>> GetLessonAsync(int pageNumber, int pageSize);
        Task<Lesson> GetByIdAsync(Guid uid);
        Task<Guid> CreateAsync(Lesson lesson);
        Task<bool> UpdateAsync(Lesson lesson);
        Task<bool> DeleteAsync(Guid uid);
    }
}
