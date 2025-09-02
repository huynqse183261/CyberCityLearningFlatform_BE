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
    public class AssignmentRepo: GenericRepository<Assignment>
    {
        public AssignmentRepo() { }
        public AssignmentRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<Assignment> GetAllAsync(bool descending = true)
        {
            var query = _context.Assignments.AsQueryable();
            return descending
                ? query.OrderByDescending(u => u.CreatedAt)
                : query.OrderBy(u => u.CreatedAt);
        }
        public async Task<List<Assignment>> GetByCourseUidAsync(Guid courseUid)
        {
            return await _context.Assignments.Where(a => a.CourseUid == courseUid).ToListAsync();
        }
    }
}
