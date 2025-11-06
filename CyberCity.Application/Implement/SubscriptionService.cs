using System;
using System.Linq;
using System.Threading.Tasks;
using CyberCity.Application.Interface;
using CyberCity.DTOs.Subscriptions;
using CyberCity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CyberCity.Application.Implement
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly OrderRepo _orderRepo;
        private readonly PaymentRepo _paymentRepo;
        private readonly PricingPlanRepo _pricingPlanRepo;
        private readonly ModuleRepo _moduleRepo;
        private readonly ILogger<SubscriptionService> _logger;
        private const int MAX_FREE_MODULES = 2;

        public SubscriptionService(
            OrderRepo orderRepo,
            PaymentRepo paymentRepo,
            PricingPlanRepo pricingPlanRepo,
            ModuleRepo moduleRepo,
            ILogger<SubscriptionService> logger)
        {
            _orderRepo = orderRepo;
            _paymentRepo = paymentRepo;
            _pricingPlanRepo = pricingPlanRepo;
            _moduleRepo = moduleRepo;
            _logger = logger;
        }

        public async Task<SubscriptionAccessDto> CheckUserSubscriptionAccessAsync(string userUid)
        {
            try
            {
                // Find active order with paid and approved status
                var activeOrder = await _orderRepo.GetAllAsync()
                    .Include(o => o.PlanU)
                    .Where(o => o.UserUid == userUid
                               && o.PaymentStatus == "paid"
                               && o.ApprovalStatus == "approved"
                               && (o.EndAt == null || o.EndAt >= DateTime.Now))
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync();

                if (activeOrder == null)
                {
                    _logger.LogInformation("No active subscription found for user {UserUid}", userUid);
                    return new SubscriptionAccessDto
                    {
                        HasAccess = false,
                        CanViewAllModules = false,
                        MaxFreeModules = MAX_FREE_MODULES,
                        SubscriptionInfo = null
                    };
                }

                // Verify payment exists and is completed
                var payment = await _paymentRepo.GetAllAsync()
                    .Where(p => p.OrderUid == activeOrder.Uid
                               && (p.Status == "paid" || p.Status == "completed"))
                    .FirstOrDefaultAsync();

                if (payment == null)
                {
                    _logger.LogWarning("Order {OrderUid} has paid status but no completed payment found", activeOrder.Uid);
                    return new SubscriptionAccessDto
                    {
                        HasAccess = false,
                        CanViewAllModules = false,
                        MaxFreeModules = MAX_FREE_MODULES,
                        SubscriptionInfo = null
                    };
                }

                // Calculate days remaining
                int? daysRemaining = null;
                if (activeOrder.EndAt.HasValue)
                {
                    var timeSpan = activeOrder.EndAt.Value - DateTime.Now;
                    daysRemaining = Math.Max(0, (int)Math.Ceiling(timeSpan.TotalDays));
                }

                _logger.LogInformation("Active subscription found for user {UserUid}, order {OrderUid}", userUid, activeOrder.Uid);

                return new SubscriptionAccessDto
                {
                    HasAccess = true,
                    CanViewAllModules = true,
                    MaxFreeModules = MAX_FREE_MODULES,
                    SubscriptionInfo = new SubscriptionInfoDto
                    {
                        Active = true,
                        OrderUid = activeOrder.Uid,
                        PlanUid = activeOrder.PlanUid,
                        PlanName = activeOrder.PlanU?.PlanName ?? "Unknown Plan",
                        StartAt = activeOrder.StartAt,
                        EndAt = activeOrder.EndAt,
                        DaysRemaining = daysRemaining
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking subscription access for user {UserUid}", userUid);
                throw;
            }
        }

        public async Task<ModuleAccessDto> CheckModuleAccessAsync(string userUid, string courseUid, int moduleIndex)
        {
            try
            {
                // Check if user has active subscription
                var subscriptionAccess = await CheckUserSubscriptionAccessAsync(userUid);

                // If user has subscription, they can access all modules
                if (subscriptionAccess.HasAccess)
                {
                    return new ModuleAccessDto
                    {
                        CanAccess = true,
                        HasSubscription = true,
                        ModuleIndex = moduleIndex,
                        MaxFreeModules = MAX_FREE_MODULES,
                        Reason = null
                    };
                }

                // Without subscription, can only access first 2 modules (index 0 and 1)
                bool canAccess = moduleIndex < MAX_FREE_MODULES;

                return new ModuleAccessDto
                {
                    CanAccess = canAccess,
                    HasSubscription = false,
                    ModuleIndex = moduleIndex,
                    MaxFreeModules = MAX_FREE_MODULES,
                    Reason = canAccess 
                        ? null 
                        : "Module này chỉ dành cho học viên đã mua gói. Vui lòng mua gói để tiếp tục học."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking module access for user {UserUid}, course {CourseUid}, module {ModuleIndex}", 
                    userUid, courseUid, moduleIndex);
                throw;
            }
        }
    }
}
