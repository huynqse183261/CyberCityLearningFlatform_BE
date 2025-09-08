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
    public class CourseRepo : GenericRepository<Course>
    {
        public CourseRepo() { }
        public CourseRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<Course> GetAllAsync(bool descending = true)
        {
            var query = _context.Courses.AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt);
        }
        public IQueryable<Course> GetAllCourseAsync()
        {
            return _context.Courses
                .Include(c => c.Modules)
                .ThenInclude(t => t.Lessons)
                .ThenInclude(s => s.Topics)
                .ThenInclude(st => st.Subtopics)
                .AsQueryable();
        }
    }
}
