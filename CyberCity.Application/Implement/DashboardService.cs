using CyberCity.Application.Interface;
using CyberCity.DTOs.Order;
using CyberCity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCity.DTOs.Dashboard;

namespace CyberCity.Application.Implement
{
    public class DashboardService : IDashboardSerivce
    {
        private readonly UserRepo _userRepo;
        private readonly OrderRepo _orderRepo;
        private readonly CourseEnrollmentRepo _courseEnrollmentRepo;
        private readonly CourseRepo _courseRepo;
        private readonly SubtopicProgressRepo _subtopicProgressRepo;
        private readonly MessageRepo _messageRepo;
        public DashboardService(UserRepo userRepo, OrderRepo orderRepo, CourseEnrollmentRepo courseEnrollmentRepo, CourseRepo courseRepo, SubtopicProgressRepo subtopicProgressRepo, MessageRepo messageRepo)
        {
            _userRepo = userRepo;
            _orderRepo = orderRepo;
            _courseEnrollmentRepo = courseEnrollmentRepo;
            _courseRepo = courseRepo;
            _subtopicProgressRepo = subtopicProgressRepo;
            _messageRepo = messageRepo;
        }
        public async Task<int> GetTotalUsersAsync()
        {
            return await _userRepo.GetUserCountAsync();
        }
        public async Task<int> GetTotalOrdersAsync()
        {
            return await _orderRepo.getOrdertotalAsync();
        }
        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _orderRepo.TotalAmountAsync();
        }
        public async Task<int> GetTotalApprovalPendingAsync()
        {
            return await _orderRepo.TotalapproveStatusAsync();
        }

        public async Task<object> GetRevenueAnalyticsAsync()
        {
            var totalRevenue = await _orderRepo.TotalAmountAsync();
            var totalOrders = await _orderRepo.getOrdertotalAsync();
            return new { totalRevenue, totalOrders };
        }

        public async Task<object> GetUserAnalyticsAsync()
        {
            var totalUsers = await _userRepo.GetUserCountAsync();
            var totalTeachers = await _userRepo.GetAllAsync().CountAsync(x => x.Role == "teacher");
            var totalStudents = await _userRepo.GetAllAsync().CountAsync(x => x.Role == "student");
            return new { totalUsers, totalTeachers, totalStudents };

        }

        public async Task<object> GetCourseAnalyticsAsync()
        {
            var totalCourses = await _courseRepo.GetAllAsync().CountAsync();
            var totalEnrollments = await _courseEnrollmentRepo.GetAll().CountAsync();
            // Có thể thêm các thống kê khác
            return new { totalCourses, totalEnrollments };
        }

        public async Task<object> GetLearningProgressAnalyticsAsync()
        {
            // Get all SubtopicProgress records
            var allProgress = await _subtopicProgressRepo.GetAllAsync().ToListAsync();

            var total = allProgress.Count;
            var completed = allProgress.Count(x => x.IsCompleted == true);
            var inProgress = allProgress.Count(x => x.IsCompleted == false);

            return new { total, completed, inProgress };
        }

        public async Task<List<OrderCountByMonthDto>> GetOrderCountByMonthAsync(int year)
        {
            return await _orderRepo.GetOrderCountByMonthAsync(year);
        }

        // New: recent orders for dashboard
        public async Task<List<RecentOrderDto>> GetRecentOrdersAsync(int count = 10)
        {
            var orders = await _orderRepo.GetRecentOrdersAsync(count);
            return orders.Select(o => new RecentOrderDto
            {
                Uid = o.Uid,
                UserUid = o.UserUid,
                Username = o.UserU?.Username,
                Email = o.UserU?.Email,
                PlanName = o.PlanU?.PlanName,
                Amount = o.Amount,
                PaymentStatus = o.PaymentStatus,
                ApprovalStatus = o.ApprovalStatus,
                CreatedAt = o.CreatedAt
            }).ToList();
        }

        // New: recent activities (users, orders, enrollments, messages)
        public async Task<List<ActivityDto>> GetRecentActivitiesAsync(int count = 20)
        {
            var activities = new List<ActivityDto>();

            // recent users
            var recentUsers = await _userRepo.GetAllAsync().Take(count).ToListAsync();
            activities.AddRange(recentUsers.Where(u => u.CreatedAt.HasValue).Select(u => new ActivityDto
            {
                Type = "user",
                Title = "Người dùng mới",
                Detail = $"{(string.IsNullOrWhiteSpace(u.FullName) ? u.Username : u.FullName)} đã đăng ký",
                UserUid = u.Uid,
                RelatedUid = u.Uid,
                When = u.CreatedAt!.Value
            }));

            // recent orders
            var recentOrders = await _orderRepo.GetRecentOrdersAsync(count);
            activities.AddRange(recentOrders.Where(o => o.CreatedAt.HasValue).Select(o => new ActivityDto
            {
                Type = "order",
                Title = "Đơn hàng mới",
                Detail = $"{o.UserU?.Username} mua gói {o.PlanU?.PlanName} ({o.Amount:C})",
                UserUid = o.UserUid,
                RelatedUid = o.Uid,
                When = o.CreatedAt!.Value
            }));

            // recent enrollments
            var recentEnrollments = await _courseEnrollmentRepo.GetAll().Take(count).ToListAsync();
            activities.AddRange(recentEnrollments.Where(e => e.EnrolledAt.HasValue).Select(e => new ActivityDto
            {
                Type = "enrollment",
                Title = "Đăng ký khóa học",
                Detail = $"User {e.UserUid} đã đăng ký khóa {e.CourseUid}",
                UserUid = e.UserUid,
                RelatedUid = e.Uid,
                When = e.EnrolledAt!.Value
            }));

            // recent messages
            var messages = await _messageRepo.GetAllAsync().Take(count).ToListAsync();
            activities.AddRange(messages.Where(m => m.SentAt.HasValue).Select(m => new ActivityDto
            {
                Type = "message",
                Title = "Tin nhắn mới",
                Detail = $"User {m.SenderUid} đã gửi tin nhắn",
                UserUid = m.SenderUid,
                RelatedUid = m.Uid,
                When = m.SentAt!.Value
            }));

            return activities.OrderByDescending(a => a.When).Take(count).ToList();
        }

        // New: quick stats
        public async Task<QuickStatsDto> GetQuickStatsAsync()
        {
            // Execute sequentially to avoid concurrent DbContext usage across scoped repos
            var totalUsers = await _userRepo.GetUserCountAsync();
            var totalOrders = await _orderRepo.getOrdertotalAsync();
            var totalRevenue = await _orderRepo.TotalAmountAsync();
            var totalPending = await _orderRepo.TotalapproveStatusAsync();
            var totalCourses = await _courseRepo.GetAllAsync().CountAsync();
            var totalEnrollments = await _courseEnrollmentRepo.GetAll().CountAsync();

            return new QuickStatsDto
            {
                TotalUsers = totalUsers,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TotalApprovalPending = totalPending,
                TotalCourses = totalCourses,
                TotalEnrollments = totalEnrollments
            };
        }
    }
}
