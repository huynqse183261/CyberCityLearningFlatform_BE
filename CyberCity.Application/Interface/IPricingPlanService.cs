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
        Task<PricingPlanDto> GetPricingPlanByIdAsync(string id);
        Task<PricingPlanDto> CreatePricingPlanAsync(CreatePricingPlanDto createDto);
        Task<PricingPlanDto> UpdatePricingPlanAsync(string id, UpdatePricingPlanDto updateDto);
        Task<bool> DeletePricingPlanAsync(string id);
        Task<bool> PricingPlanExistsAsync(string id);
        Task<bool> PlanNameExistsAsync(string planName, string? excludeId = null);
    }
}