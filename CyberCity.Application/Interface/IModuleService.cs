using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Courses;
using CyberCity.DTOs.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface IModuleService
    {
        Task<PagedResult<ModuleDetailDto>> GetModuleAsync(int pageNumber, int pageSize);
        Task<Module> GetByIdAsync(Guid uid);
        Task<Guid> CreateAsync(Module module);
        Task<bool> UpdateAsync(Module module);
        Task<bool> DeleteAsync(Guid uid);
    }
}
