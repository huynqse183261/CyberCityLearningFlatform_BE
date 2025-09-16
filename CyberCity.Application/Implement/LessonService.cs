using AutoMapper;
using CloudinaryDotNet.Actions;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Lessons;
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
    public class LessonService: ILessonService
    {
        private readonly LessonRepo _lessonRepo;
        private readonly IMapper _mapper;


        public LessonService(LessonRepo lessonRepo, IMapper mapper)
        {
            _lessonRepo = lessonRepo;
            _mapper = mapper;
        }

        public async Task<PagedResult<LessonDetailResponse>> GetLessonAsync(int page, int pageSize)
        {
            var query = _lessonRepo.GetAllAsync();
            var totalItems = await query.CountAsync();

            var courses = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedCourses = courses.Select(course => _mapper.Map<LessonDetailResponse>(course)).ToList();

            return new PagedResult<LessonDetailResponse>
            {
                Items = mappedCourses,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }

        public async Task<Lesson> GetByIdAsync(Guid uid)
        {
            return await _lessonRepo.GetByIdAsync(uid);
        }

        public async Task<Guid> CreateAsync(Lesson lesson)
        {
            lesson.Uid = Guid.NewGuid();
            lesson.CreatedAt = DateTime.Now;
            var result = await _lessonRepo.CreateAsync(lesson);
            return result > 0 ? lesson.Uid : Guid.Empty;
        }

        public async Task<bool> UpdateAsync(Lesson lesson)
        {
            var result = await _lessonRepo.UpdateAsync(lesson);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(Guid uid)
        {
            var existing = await _lessonRepo.GetByIdAsync(uid);
            if (existing == null) return false;
            return await _lessonRepo.RemoveAsync(existing);
        }

    }
}
