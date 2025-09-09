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

        public async Task<CourseProgress?> GetByCourseAndStudentAsync(Guid courseId, Guid studentId)
        {
            return await _context.CourseProgresses
                .Include(x => x.StudentU)
                .FirstOrDefaultAsync(x => x.CourseUid == courseId && x.StudentUid == studentId);
        }

        public async Task<List<CourseProgress>> GetByCourseAsync(Guid courseId)
        {
            return await _context.CourseProgresses
                .Include(x => x.StudentU)
                .Where(x => x.CourseUid == courseId)
                .ToListAsync();
        }
    }
}
