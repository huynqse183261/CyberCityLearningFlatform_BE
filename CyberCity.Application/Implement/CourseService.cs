using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Courses;
using CyberCity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyberCity.Application.Implement
{
    public class CourseService : ICourseService
    {
        private readonly CourseRepo _courseRepo;
        private readonly TopicRepo _topicRepo;
        private readonly SubtopicRepo _subtopicRepo;
        private readonly LessonRepo _lessonRepo;
        private readonly IMapper _mapper;

        public CourseService(CourseRepo courseRepo, TopicRepo topicRepo, SubtopicRepo subtopicRepo, LessonRepo lessonRepo, IMapper mapper)
        {
            _courseRepo = courseRepo;
            _topicRepo = topicRepo;
            _subtopicRepo = subtopicRepo;
            _lessonRepo = lessonRepo;
            _mapper = mapper;
        }

        public async Task<PagedResult<Course>> GetCoursesAsync(int pageNumber, int pageSize, string? level = null, bool descending = true)
        {
            var query = _courseRepo.GetAllAsync(descending);
            if (!string.IsNullOrWhiteSpace(level))
            {
                query = query.Where(c => c.Level == level);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Course>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }
        public async Task<PagedResult<CourseOutlineResponse>> GetAllOutline(int page, int pageSize)
        {
            var query = _courseRepo.GetAllCourseAsync();
            var totalItems = await query.CountAsync();

            var courses = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedCourses = courses.Select(course => _mapper.Map<CourseOutlineResponse>(course)).ToList();

            return new PagedResult<CourseOutlineResponse>
            {
                Items = mappedCourses,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }
        

        public async Task<Course> GetByIdAsync(string uid)
        {
            return await _courseRepo.GetByIdAsync(uid);
        }

        public async Task<string> CreateAsync(Course course)
        {
            course.Uid = Guid.NewGuid().ToString();
            course.CreatedAt = DateTime.Now;
            var result = await _courseRepo.CreateAsync(course);
            return result > 0 ? course.Uid : null;
        }

        public async Task<bool> UpdateAsync(Course course)
        {
            var updated = await _courseRepo.UpdateAsync(course);
            return updated > 0;
        }

        public async Task<bool> DeleteAsync(string uid)
        {
            var existing = await _courseRepo.GetByIdAsync(uid);
            if (existing == null) return false;
            return await _courseRepo.RemoveAsync(existing);
        }

    }
}