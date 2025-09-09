using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.DTOs.Courses;
using CyberCity.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Implement
{
    public class CourseProgressService: ICourseProgressService
    {
        private readonly CourseProgressRepo _repo;
        private readonly IMapper _mapper;

        public CourseProgressService(CourseProgressRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<CourseProgressMeDto?> GetMyProgressAsync(Guid courseId, Guid studentId)
        {
            var progress = await _repo.GetByCourseAndStudentAsync(courseId, studentId);
            return progress == null ? null : _mapper.Map<CourseProgressMeDto>(progress);
        }

        public async Task<List<CourseProgressOverviewItemDto>> GetCourseProgressOverviewAsync(Guid courseId)
        {
            var progresses = await _repo.GetByCourseAsync(courseId);
            return progresses.Select(x => _mapper.Map<CourseProgressOverviewItemDto>(x)).ToList();
        }
    }
}
