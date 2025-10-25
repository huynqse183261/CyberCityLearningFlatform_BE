using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Modules;

namespace CyberCity.AutoMapper
{
    public class ModuleProfileExtended : Profile
    {
        public ModuleProfileExtended()
        {
            CreateMap<Module, ModuleDto>();
            CreateMap<ModuleDto, Module>();
        }
    }
}

