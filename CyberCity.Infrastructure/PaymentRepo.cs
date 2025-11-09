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

		// Partial update: only update status and paid_at to avoid unintended column writes
		public async Task<int> UpdateStatusAsync(string paymentUid, string status, DateTime? paidAt)
		{
			var entity = new Payment { Uid = paymentUid };
			_context.Attach(entity);
			entity.Status = status;
			entity.PaidAt = paidAt;
			_context.Entry(entity).Property(x => x.Status).IsModified = true;
			_context.Entry(entity).Property(x => x.PaidAt).IsModified = true;
			return await _context.SaveChangesAsync();
		}
    }
}
    