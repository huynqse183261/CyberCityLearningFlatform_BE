using AutoMapper;
using CloudinaryDotNet.Actions;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Lessons;
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
    public class TopicService: ITopicService
    {
        private readonly TopicRepo _topicRepo;
        private readonly IMapper _mapper;
        public TopicService(TopicRepo topicRepo, IMapper mapper)
        {
            _topicRepo = topicRepo;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(Topic topic)
        {
            topic.Uid = Guid.NewGuid().ToString();
            topic.CreatedAt = DateTime.Now;
            var result = await _topicRepo.CreateAsync(topic);
            return result > 0 ? Guid.Parse(topic.Uid) : Guid.Empty;
        }

        public async Task<bool> DeleteAsync(Guid uid)
        {
            var existing = await _topicRepo.GetByIdAsync(uid);
            if (existing == null) return false;
            return await _topicRepo.RemoveAsync(existing);
        }

        public async Task<Topic> GetByIdAsync(Guid uid)
        {
            return await _topicRepo.GetByIdAsync(uid);
        }

        public async Task<PagedResult<TopicDetailDto>> GetTopicAsync(int page, int pageSize)
        {
            var query = _topicRepo.GetAllAsync();
            var totalItems = await query.CountAsync();

            var courses = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedCourses = courses.Select(course => _mapper.Map<TopicDetailDto>(course)).ToList();

            return new PagedResult<TopicDetailDto>
            {
                Items = mappedCourses,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }

        public async Task<bool> UpdateAsync(Topic topic)
        {
            var result = await _topicRepo.UpdateAsync(topic);
            return result > 0;
        }
    }
}
