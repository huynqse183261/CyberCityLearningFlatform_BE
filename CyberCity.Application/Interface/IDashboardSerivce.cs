using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface IDashboardSerivce
    {
        public Task<int> GetTotalUsersAsync();
        public Task<int> GetTotalOrdersAsync();
        public Task<decimal> GetTotalRevenueAsync();
        public Task<int> GetTotalApprovalPendingAsync();
        public Task<object> GetRevenueAnalyticsAsync();
        public Task<object> GetUserAnalyticsAsync();
        public Task<object> GetCourseAnalyticsAsync();
        public Task<object> GetLearningProgressAnalyticsAsync();


    }
}
