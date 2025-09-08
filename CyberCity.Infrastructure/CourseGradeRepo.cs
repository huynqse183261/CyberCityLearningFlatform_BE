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
    public class CourseGradeRepo: GenericRepository<CourseGrade>
    {
        public CourseGradeRepo() { }
        public CourseGradeRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<CourseGrade> GetAllAsync(bool descending = true)
        {
            var query = _context.CourseGrades.AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.GradedBy)
                : query.OrderBy(c => c.GradedBy);
        }
        public async Task<List<CourseGrade>> GetByCourseEnrollmentUidAsync(Guid courseEnrollmentUid)
        {
            return await _context.CourseGrades.Where(c => c.CourseUid == courseEnrollmentUid).ToListAsync();
        }
    }
}
