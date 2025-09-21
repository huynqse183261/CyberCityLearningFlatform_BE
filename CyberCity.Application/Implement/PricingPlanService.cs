using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs.PricingPlans;
using CyberCity.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Application.Implement
{
    public class PricingPlanService : IPricingPlanService
    {
        private readonly PricingPlanRepo _pricingPlanRepo;
        private readonly IMapper _mapper;

        public PricingPlanService(PricingPlanRepo pricingPlanRepo, IMapper mapper)
        {
            _pricingPlanRepo = pricingPlanRepo;
            _mapper = mapper;
        }

        public async Task<List<PricingPlanDto>> GetAllPricingPlansAsync(bool descending = true)
        {
            var pricingPlans = await _pricingPlanRepo.GetAllWithOrderCountAsync(descending);
            return pricingPlans.Select(p => _mapper.Map<PricingPlanDto>(p)).ToList();
        }

        public async Task<PricingPlanDto> GetPricingPlanByIdAsync(Guid id)
        {
            var pricingPlan = await _pricingPlanRepo.GetByIdWithOrdersAsync(id);
            if (pricingPlan == null)
                throw new ArgumentException("Pricing plan not found");

            return _mapper.Map<PricingPlanDto>(pricingPlan);
        }

        public async Task<PricingPlanDto> CreatePricingPlanAsync(CreatePricingPlanDto createDto)
        {
            // Check if plan name already exists
            var existingPlan = await _pricingPlanRepo.GetByNameAsync(createDto.PlanName);
            if (existingPlan != null)
                throw new ArgumentException("Plan name already exists");

            var pricingPlan = _mapper.Map<PricingPlan>(createDto);
            pricingPlan.Uid = Guid.NewGuid();
            pricingPlan.CreatedAt = DateTime.Now;

            await _pricingPlanRepo.CreateAsync(pricingPlan);

            return _mapper.Map<PricingPlanDto>(pricingPlan);
        }

        public async Task<PricingPlanDto> UpdatePricingPlanAsync(Guid id, UpdatePricingPlanDto updateDto)
        {
            var existingPlan = await _pricingPlanRepo.GetByIdAsync(id);
            if (existingPlan == null)
                throw new ArgumentException("Pricing plan not found");

            // Check if new plan name already exists (if changing name)
            if (!string.IsNullOrEmpty(updateDto.PlanName) && 
                updateDto.PlanName != existingPlan.PlanName)
            {
                var nameExists = await PlanNameExistsAsync(updateDto.PlanName, id);
                if (nameExists)
                    throw new ArgumentException("Plan name already exists");
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateDto.PlanName))
                existingPlan.PlanName = updateDto.PlanName;
            
            if (updateDto.Price.HasValue)
                existingPlan.Price = updateDto.Price.Value;
            
            if (updateDto.DurationDays.HasValue)
                existingPlan.DurationDays = updateDto.DurationDays.Value;
            
            if (updateDto.Features != null)
                existingPlan.Features = updateDto.Features;

            await _pricingPlanRepo.UpdateAsync(existingPlan);

            return _mapper.Map<PricingPlanDto>(existingPlan);
        }

        public async Task<bool> DeletePricingPlanAsync(Guid id)
        {
            var pricingPlan = await _pricingPlanRepo.GetByIdAsync(id);
            if (pricingPlan == null)
                return false;

            // Check if pricing plan is being used in any orders
            var isBeingUsed = await _pricingPlanRepo.IsBeingUsedAsync(id);
            if (isBeingUsed)
                throw new InvalidOperationException("Cannot delete pricing plan that is being used in orders");

            await _pricingPlanRepo.RemoveAsync(pricingPlan);
            return true;
        }

        public async Task<bool> PricingPlanExistsAsync(Guid id)
        {
            var pricingPlan = await _pricingPlanRepo.GetByIdAsync(id);
            return pricingPlan != null;
        }

        public async Task<bool> PlanNameExistsAsync(string planName, Guid? excludeId = null)
        {
            var existingPlan = await _pricingPlanRepo.GetByNameAsync(planName);
            if (existingPlan == null)
                return false;

            // If excludeId is provided, ignore that specific plan (for updates)
            return excludeId == null || existingPlan.Uid != excludeId;
        }
    }
}