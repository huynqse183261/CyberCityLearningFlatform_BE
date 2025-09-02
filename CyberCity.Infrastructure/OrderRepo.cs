using CyberCity.Infrastructure.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.Doman.DBcontext;
using CyberCity.Doman.Models;

namespace CyberCity.Infrastructure
{
    public class OrderRepo: GenericRepository<Order>
    {
        public OrderRepo() { }
        public OrderRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<Order> GetAllAsync(bool descending = true)
        {
            var query = _context.Orders.AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.ApprovalStatus)
                : query.OrderBy(c => c.ApprovalStatus);
        }
    }
}
