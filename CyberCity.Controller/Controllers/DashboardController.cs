using CyberCity.Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controller.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardSerivce _dashboardSerivce;
        public DashboardController(IDashboardSerivce dashboardSerivce)
        {
            _dashboardSerivce = dashboardSerivce;
        }
        [HttpGet("total-users")]
        public async Task<IActionResult> GetTotalUsersAsync()
        {
            var totalUsers = await _dashboardSerivce.GetTotalUsersAsync();
            return Ok(new { totalUsers });
        }
        [HttpGet("total-orders")]
        public async Task<IActionResult> GetTotalOrdersAsync()
        {
            var totalOrders = await _dashboardSerivce.GetTotalOrdersAsync();
            return Ok(new { totalOrders });
        }
        [HttpGet("total-revenue")]
        public async Task<IActionResult> GetTotalRevenueAsync()
        {
            var totalRevenue = await _dashboardSerivce.GetTotalRevenueAsync();
            return Ok(new { totalRevenue });
        }
        [HttpGet("total-approval-pending")]
        public async Task<IActionResult> GetTotalApprovalPendingAsync()
        {
            var totalApprovalPending = await _dashboardSerivce.GetTotalApprovalPendingAsync();
            return Ok(new { totalApprovalPending });
        }
        [HttpGet("revenue-analytics")]
        public async Task<IActionResult> GetRevenueAnalyticsAsync()
        {
            var revenueAnalytics = await _dashboardSerivce.GetRevenueAnalyticsAsync();
            return Ok(revenueAnalytics);
        }
        [HttpGet("user-analytics")]
        public async Task<IActionResult> GetUserAnalyticsAsync()
        {
            var userAnalytics = await _dashboardSerivce.GetUserAnalyticsAsync();
            return Ok(userAnalytics);
        }
        [HttpGet("course-analytics")]
        public async Task<IActionResult> GetCourseAnalyticsAsync()
        {
            var courseAnalytics = await _dashboardSerivce.GetCourseAnalyticsAsync();
            return Ok(courseAnalytics);
        }
        [HttpGet("learning-progress-analytics")]
        public async Task<IActionResult> GetLearningProgressAnalyticsAsync()
        {
            var learningProgressAnalytics = await _dashboardSerivce.GetLearningProgressAnalyticsAsync();
            return Ok(learningProgressAnalytics);
        }
        [HttpGet("overview")]
        public async Task<IActionResult> GetDashboardOverviewAsync()
        {
            var totalUsers = await _dashboardSerivce.GetTotalUsersAsync();
            var totalOrders = await _dashboardSerivce.GetTotalOrdersAsync();
            var totalRevenue = await _dashboardSerivce.GetTotalRevenueAsync();
            var totalApprovalPending = await _dashboardSerivce.GetTotalApprovalPendingAsync();
            var revenueAnalytics = await _dashboardSerivce.GetRevenueAnalyticsAsync();
            var userAnalytics = await _dashboardSerivce.GetUserAnalyticsAsync();
            var courseAnalytics = await _dashboardSerivce.GetCourseAnalyticsAsync();
            var learningProgressAnalytics = await _dashboardSerivce.GetLearningProgressAnalyticsAsync();

            return Ok(new
            {
                totalUsers,
                totalOrders,
                totalRevenue,
                totalApprovalPending,
                revenueAnalytics,
                userAnalytics,
                courseAnalytics,
                learningProgressAnalytics
            });
        }

    }
}
