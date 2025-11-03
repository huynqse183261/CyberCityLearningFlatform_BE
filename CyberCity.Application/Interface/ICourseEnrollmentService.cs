using CyberCity.DTOs.Enrollments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface ICourseEnrollmentService
    {
        Task<bool> EnrollAsync(string courseId, string studentId);
        Task<List<EnrollmentResponse>> GetMyEnrollmentsAsync(string studentId);
        Task<List<CourseEnrollmentResponse>> GetEnrollmentsByCourseAsync(string courseId);
    }
}
