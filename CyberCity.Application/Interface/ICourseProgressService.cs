using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.DTOs.Courses;

namespace CyberCity.Application.Interface
{
    public interface ICourseProgressService
    {

        Task<CourseProgressMeDto?> GetMyProgressAsync(Guid courseId, Guid studentId);
        Task<List<CourseProgressOverviewItemDto>> GetCourseProgressOverviewAsync(Guid courseId);
    }
}
