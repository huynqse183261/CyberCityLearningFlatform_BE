using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.DTOs;
using CyberCity.DTOs.Subtopics;
using CyberCity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Implement
{
    public class SubtopicProgressService: ISubtopicProgressService
    {
        private readonly SubtopicProgressRepo _repo;
        private readonly IMapper _mapper;
        public SubtopicProgressService(SubtopicProgressRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<SubtopicProgressDto>> GetByStudentAsync(Guid studentId)
        {
            var list = await _repo.GetByStudentAsync(studentId);
            return _mapper.Map<List<SubtopicProgressDto>>(list);
        }

        public async Task<List<SubtopicProgressDto>> GetBySubtopicAndStudentAsync(Guid courseId, Guid studentId)
        {
            var list = await _repo.GetBySubtopicAndStudentAsync(courseId, studentId);
            return _mapper.Map<List<SubtopicProgressDto>>(list);
        }

        public async Task<SubtopicProgressDto> MarkCompleteAsync(Guid subtopicId, Guid studentId)
        {
            await _repo.MarkCompleteAsync(subtopicId, studentId);
            var progress = await _repo.GetBySubtopicAndStudentAsync(subtopicId, studentId);
            return _mapper.Map<SubtopicProgressDto>(progress);
        }
        public async Task<PagedResult<SubtopicProgressDto>> GetAllsubtopicProgressAsync(int page, int pageSize)
        {
            var query = _repo.GetAllAsync();
            var totalItems = await query.CountAsync();

            var subtopic = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedSubtopic = subtopic.Select(course => _mapper.Map<SubtopicProgressDto>(course)).ToList();

            return new PagedResult<SubtopicProgressDto>
            {
                Items = mappedSubtopic,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }
    }
}
