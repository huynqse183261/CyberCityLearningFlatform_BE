using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Subtopics;
using CyberCity.DTOs.Topics;
using CyberCity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Implement
{
    public class SubtopicService: ISubtopicService
    {
        private readonly SubtopicRepo _subtopicRepo;
        private readonly IMapper _mapper;
        public SubtopicService(SubtopicRepo subtopicRepo, IMapper mapper)
        {
            _subtopicRepo = subtopicRepo;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(Subtopic subtopic)
        {
            subtopic.Uid = Guid.NewGuid();
            subtopic.CreatedAt = DateTime.Now;
            var result = await _subtopicRepo.CreateAsync(subtopic);
            return result > 0 ? subtopic.Uid : Guid.Empty;
        }

        public async Task<bool> DeleteAsync(Guid uid)
        {
            var existing = await _subtopicRepo.GetByIdAsync(uid);
            if (existing == null) return false;
            return await _subtopicRepo.RemoveAsync(existing);
        }

        public async Task<Subtopic> GetByIdAsync(Guid uid)
        {
           var subtopic = await _subtopicRepo.GetByIdAsync(uid);
            return subtopic;
        }

        public async Task<PagedResult<SubtopicDetailDto>> GetSubtopicAsync(int page, int pageSize)
        {
            var query = _subtopicRepo.GetAllAsync();
            var totalItems = await query.CountAsync();

            var courses = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedCourses = courses.Select(course => _mapper.Map<SubtopicDetailDto>(course)).ToList();

            return new PagedResult<SubtopicDetailDto>
            {
                Items = mappedCourses,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }

        public async Task<bool> UpdateAsync(Subtopic subtopic)
        {
            var result = await _subtopicRepo.UpdateAsync(subtopic);
            return result > 0;
        }
    }
}
