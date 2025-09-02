using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.Doman.DBcontext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;

namespace CyberCity.Infrastructure
{
    public class SubmissionRepo: GenericRepository<Submission>
    {
        public SubmissionRepo() { }
        public SubmissionRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<Submission> GetAllAsync(bool descending = true)
        {
            var query = _context.Submissions.AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.SubmittedAt)
                : query.OrderBy(c => c.SubmittedAt);
        }
    }
}
