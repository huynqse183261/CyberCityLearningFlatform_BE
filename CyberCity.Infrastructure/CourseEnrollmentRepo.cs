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
        public IQueryable<CourseEnrollment> GetAllAsync(bool descending = true)
        {
            var query = _context.CourseEnrollments.AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.EnrolledAt)
                : query.OrderBy(c => c.EnrolledAt);
        }
        public async Task<List<CourseEnrollment>> GetByCourseUidAsync(Guid courseUid)
        {
            return await _context.CourseEnrollments.Where(c => c.CourseUid == courseUid).ToListAsync();
        }
    }
}
