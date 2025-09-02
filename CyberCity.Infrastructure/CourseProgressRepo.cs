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
    public class CourseProgressRepo : GenericRepository<CourseProgress>
    {
        public CourseProgressRepo() { }
        public CourseProgressRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<CourseProgress> GetAllAsync(bool descending = true)
        {
            var query = _context.CourseProgresses.AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.LastAccessedAt)
                : query.OrderBy(c => c.LastAccessedAt);
        }
        public async Task<List<CourseProgress>> GetByCourseEnrollmentUidAsync(Guid courseEnrollmentUid)
        {
            return await _context.CourseProgresses.Where(c => c.CourseUid == courseEnrollmentUid).ToListAsync();
        }
    }
}
