using CyberCity.Application.Interface;
using CyberCity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Implement
{
    public class DashboardService : IDashboardSerivce
    {
        private readonly UserRepo _userRepo;
        private readonly OrderRepo _orderRepo;
        private readonly CourseEnrollmentRepo _courseEnrollmentRepo;
        private readonly CourseRepo _courseRepo;
        private readonly SubtopicProgressRepo _subtopicProgressRepo;
        public DashboardService(UserRepo userRepo, OrderRepo orderRepo, CourseEnrollmentRepo courseEnrollmentRepo, CourseRepo courseRepo, SubtopicProgressRepo subtopicProgressRepo)
        {
            _userRepo = userRepo;
            _orderRepo = orderRepo;
            _courseEnrollmentRepo = courseEnrollmentRepo;
            _courseRepo = courseRepo;
            _subtopicProgressRepo = subtopicProgressRepo;
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
    }
}
