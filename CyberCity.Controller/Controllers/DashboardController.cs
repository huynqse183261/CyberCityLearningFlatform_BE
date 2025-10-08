using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.DTOs;
using CyberCity.DTOs.Courses;
using CyberCity.DTOs.UserAccount;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using CyberCity.DTOs.Dashboard;
using CyberCity.DTOs.Order;

namespace CyberCity.Controller.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardSerivce _dashboardSerivce;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ICourseService _courseService;
        public DashboardController(IDashboardSerivce dashboardSerivce, IUserService userService, IMapper mapper,ICourseService courseService)
        {
            _dashboardSerivce = dashboardSerivce;
            _userService = userService;
            _courseService = courseService;
            _mapper = mapper;
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
        [HttpGet("orders-by-month")]
        public async Task<IActionResult> GetOrderCountByMonthAsync([FromQuery] int year)
        {
            var data = await _dashboardSerivce.GetOrderCountByMonthAsync(year);
            return Ok(data);
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
        //---------------------------------------------------------------//
        [HttpGet("users")]
        public async Task<ActionResult<PagedResult<UserAccountDTO>>> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, [FromQuery] bool descending = true)
        {
            var result = await _userService.GetAllAccounts(pageNumber, pageSize, descending);
            var dto = new PagedResult<UserAccountDTO>
            {
                Items = result.Items.ConvertAll(u => _mapper.Map<UserAccountDTO>(u)),
                TotalItems = result.TotalItems,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
            return Ok(dto);
        }
        [HttpPut("users/{id}/status")]
        public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusDto request)
        {
            if (id == Guid.Empty) return BadRequest("Invalid id");
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            user.Status = request?.Status;
            var updated = await _userService.UpdateAccount(user);
            if (updated <= 0) return BadRequest("Cannot update status");
            return NoContent();
        }
        //---------------------------------------------------------------//
        
        // New endpoints
        [HttpGet("recent-orders")]
        public async Task<ActionResult<List<RecentOrderDto>>> GetRecentOrders([FromQuery] int count = 10)
        {
            var orders = await _dashboardSerivce.GetRecentOrdersAsync(count);
            return Ok(orders);
        }

        [HttpGet("recent-activities")]
        public async Task<ActionResult<List<ActivityDto>>> GetRecentActivities([FromQuery] int count = 20)
        {
            var activities = await _dashboardSerivce.GetRecentActivitiesAsync(count);
            return Ok(activities);
        }

        [HttpGet("quick-stats")]
        public async Task<ActionResult<QuickStatsDto>> GetQuickStats()
        {
            var stats = await _dashboardSerivce.GetQuickStatsAsync();
            return Ok(stats);
        }

    }
}
