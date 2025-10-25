using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Courses;
using CyberCity.DTOs.Modules;
using CyberCity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Implement
{
    public class ModuleService: IModuleService
    {
        private readonly ModuleRepo _moduleRepo;
        private readonly IMapper _mapper;
        public ModuleService(ModuleRepo moduleRepo, IMapper mapper)
        {
            _moduleRepo = moduleRepo;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(Module module)
        {
            module.Uid = Guid.NewGuid().ToString();
            module.CreatedAt = DateTime.Now;
            var result = await _moduleRepo.CreateAsync(module);
            return result > 0 ? Guid.Parse(module.Uid) : Guid.Empty;
        }

        public async Task<bool> DeleteAsync(Guid uid)
        {
            var existing = await _moduleRepo.GetByIdAsync(uid);
            if (existing == null) return false;
            return await _moduleRepo.RemoveAsync(existing);
        }

        public async Task<Module> GetByIdAsync(Guid uid)
        {
          return await _moduleRepo.GetByIdAsync(uid);
        }

        public async Task<PagedResult<ModuleDetailDto>> GetModuleAsync(int page, int pageSize)
        {
            var query = _moduleRepo.GetAllAsync();
            var totalItems = await query.CountAsync();

            var courses = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedCourses = courses.Select(course => _mapper.Map<ModuleDetailDto>(course)).ToList();

            return new PagedResult<ModuleDetailDto>
            {
                Items = mappedCourses,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }

        public async Task<bool> UpdateAsync(Module module)
        {
            var result = await _moduleRepo.UpdateAsync(module);
            return result > 0;
        }
    }
}
