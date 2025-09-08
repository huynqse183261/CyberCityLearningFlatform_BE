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
    public class ApprovalLogRepo: GenericRepository<ApprovalLog>
    {
        public ApprovalLogRepo() { }
        public ApprovalLogRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<ApprovalLog> GetAllAsync(bool descending = true)
        {
            var query = _context.ApprovalLogs.AsQueryable();
            return descending
                ? query.OrderByDescending(u => u.CreatedAt)
                : query.OrderBy(u => u.CreatedAt);
        }
        public async Task<List<ApprovalLog>> GetByOrderUidAsync(Guid orderUid)
        {
            return await _context.ApprovalLogs.Where(a => a.OrderUid == orderUid).ToListAsync();
        }
    }
}
