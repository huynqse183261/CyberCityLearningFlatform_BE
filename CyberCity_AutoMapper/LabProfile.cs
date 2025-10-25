using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Labs;

namespace CyberCity.AutoMapper
{
    public class LabProfile : Profile
    {
        public LabProfile()
        {
            CreateMap<Lab, LabDto>();
            CreateMap<LabDto, Lab>();

            CreateMap<LabComponent, LabComponentDto>();
            CreateMap<LabComponentDto, LabComponent>();
        }
    }
}

