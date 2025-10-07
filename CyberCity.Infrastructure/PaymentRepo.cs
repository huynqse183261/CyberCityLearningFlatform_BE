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
    public class PaymentRepo: GenericRepository<Payment>
    {
        public PaymentRepo() { }
        public PaymentRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<Payment> GetAllAsync(bool descending = true)
        {
            var query = _context.Payments
                .OrderByDescending(t => t.CreatedAt)
                .AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt);
        }
    }
}
    