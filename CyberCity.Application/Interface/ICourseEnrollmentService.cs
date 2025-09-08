using CyberCity.DTOs.Enrollments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface ICourseEnrollmentService
    {
        Task<bool> EnrollAsync(Guid courseId, Guid studentId);
        Task<List<EnrollmentDto>> GetMyEnrollmentsAsync(Guid studentId);
        Task<List<CourseEnrollmentDto>> GetEnrollmentsByCourseAsync(Guid courseId);
    }
}
