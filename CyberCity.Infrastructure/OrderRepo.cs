using CyberCity.Infrastructure.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.Doman.DBcontext;
using CyberCity.Doman.Models;
using Microsoft.EntityFrameworkCore;

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
        public async Task<int> getOrdertotalAsync()
        {
            return await _context.Orders.CountAsync();
        }
        public async Task<decimal> TotalAmountAsync()
        {
            return await _context.Orders.SumAsync(o => o.Amount);
        }
        public async Task<int> TotalapproveStatusAsync()
        {
            return await _context.Orders.CountAsync(o => o.ApprovalStatus == "pending");
        }
    }
}
