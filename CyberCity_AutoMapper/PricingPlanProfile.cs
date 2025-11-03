using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.PricingPlans;
using System;
using System.Linq;
using System.Text.Json;

namespace CyberCity_AutoMapper
{
    public class PricingPlanProfile : Profile
    {
        public PricingPlanProfile()
        {
            CreateMap<PricingPlan, PricingPlanDto>()
                .ForMember(dest => dest.OrderCount, opt => opt.MapFrom(src => src.Orders.Count));

            CreateMap<CreatePricingPlanDto, PricingPlan>()
                .ForMember(dest => dest.Uid, opt => opt.MapFrom(_ => Guid.NewGuid().ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Orders, opt => opt.Ignore());

            CreateMap<UpdatePricingPlanDto, PricingPlan>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}