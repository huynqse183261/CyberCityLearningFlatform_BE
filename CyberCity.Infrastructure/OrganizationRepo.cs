using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.Doman.DBContext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;

namespace CyberCity.Infrastructure
{
    public class OrganizationRepo:GenericRepository<Organization>
    {
        public OrganizationRepo() { }
        public OrganizationRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<Organization> GetAllAsync(bool descending = true)
        {
            var query = _context.Organizations.OrderByDescending(t => t.CreatedAt).AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt);
        }
    }
}
