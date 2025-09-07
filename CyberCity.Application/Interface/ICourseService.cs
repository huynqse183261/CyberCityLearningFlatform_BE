using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface ICourseService
    {
        Task<PagedResult<Course>> GetCoursesAsync(int pageNumber, int pageSize, string level = null, bool descending = true);
        Task<PagedResult<CourseOutlineResponseDto>> GetAllOutline(int page, int pageSize);
        Task<Course> GetByIdAsync(Guid uid);
        Task<Guid> CreateAsync(Course course);
        Task<bool> UpdateAsync(Course course);
        Task<bool> DeleteAsync(Guid uid);
    }
}
