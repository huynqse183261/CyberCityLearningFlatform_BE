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
    public class NotificationRepo:GenericRepository<Notification>
    {
        public NotificationRepo() { }
        public NotificationRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<Notification> GetAllAsync(bool descending = true)
        {
            var query = _context.Notifications.AsQueryable();
            return descending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt);
        }
    }
}
