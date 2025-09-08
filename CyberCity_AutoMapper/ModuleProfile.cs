using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Modules;

namespace CyberCity_AutoMapper
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile()
        {
            CreateMap<Module, ModuleDetailDto>().ReverseMap();
            CreateMap<ModuleCreateDto, Module>();
            CreateMap<ModuleUpdateDto, Module>();
        }
    }
}
