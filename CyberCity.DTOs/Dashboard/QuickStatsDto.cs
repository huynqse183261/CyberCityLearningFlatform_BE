using System;

namespace CyberCity.DTOs.Dashboard
{
    public class QuickStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalApprovalPending { get; set; }
        public int TotalCourses { get; set; }
        public int TotalEnrollments { get; set; }
    }
}