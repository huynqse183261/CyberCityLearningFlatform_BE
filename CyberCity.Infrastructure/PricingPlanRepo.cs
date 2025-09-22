using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.Doman.DBcontext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<PricingPlan>> GetAllWithOrderCountAsync(bool descending = true)
        {
            var query = _context.PricingPlans
                .Include(p => p.Orders)
                .AsQueryable();

            return descending
                ? await query.OrderByDescending(p => p.CreatedAt).ToListAsync()
                : await query.OrderBy(p => p.CreatedAt).ToListAsync();
        }

        public async Task<PricingPlan> GetByIdWithOrdersAsync(Guid id)
        {
            return await _context.PricingPlans
                .Include(p => p.Orders)
                .FirstOrDefaultAsync(p => p.Uid == id);
        }

        public async Task<bool> IsBeingUsedAsync(Guid id)
        {
            return await _context.Orders.AnyAsync(o => o.PlanUid == id);
        }

        public async Task<PricingPlan> GetByNameAsync(string planName)
        {
            return await _context.PricingPlans
                .FirstOrDefaultAsync(p => p.PlanName.ToLower() == planName.ToLower());
        }
    }
}
