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
    public class PricingPlanRepo: GenericRepository<PricingPlan>
    {
        public PricingPlanRepo() { }
        public PricingPlanRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<PricingPlan> GetAllAsync(bool descending = true)
        {
            var query = _context.PricingPlans.AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt);
        }
    }
}
