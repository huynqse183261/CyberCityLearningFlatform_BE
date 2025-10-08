using CyberCity.Doman.DBcontext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Infrastructure
{
    public class CourseEnrollmentRepo: GenericRepository<CourseEnrollment>
    {
        public CourseEnrollmentRepo() { }
        public CourseEnrollmentRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<CourseEnrollment> GetAll() => _context.CourseEnrollments
            .OrderByDescending(e => e.EnrolledAt)
            .AsQueryable();

        public async Task<CourseEnrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId)
        {
            return await _context.CourseEnrollments
                .FirstOrDefaultAsync(e => e.UserUid == userId && e.CourseUid == courseId);
        }
    }
}
