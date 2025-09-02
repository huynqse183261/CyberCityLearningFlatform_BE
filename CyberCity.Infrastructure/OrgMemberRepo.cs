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
    public class OrgMemberRepo: GenericRepository<OrgMember>
    {
        public OrgMemberRepo() { }
        public OrgMemberRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<OrgMember> GetAllAsync(bool descending = true)
        {
            var query = _context.OrgMembers.AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.JoinedAt)
                : query.OrderBy(c => c.JoinedAt);
        }
    }
}
