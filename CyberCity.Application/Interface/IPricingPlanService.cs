using CyberCity.DTOs;
using CyberCity.DTOs.PricingPlans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface IPricingPlanService
    {
        Task<List<PricingPlanDto>> GetAllPricingPlansAsync(bool descending = true);
        Task<PricingPlanDto> GetPricingPlanByIdAsync(Guid id);
        Task<PricingPlanDto> CreatePricingPlanAsync(CreatePricingPlanDto createDto);
        Task<PricingPlanDto> UpdatePricingPlanAsync(Guid id, UpdatePricingPlanDto updateDto);
        Task<bool> DeletePricingPlanAsync(Guid id);
        Task<bool> PricingPlanExistsAsync(Guid id);
        Task<bool> PlanNameExistsAsync(string planName, Guid? excludeId = null);
    }
}