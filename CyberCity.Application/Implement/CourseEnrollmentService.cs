using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Enrollments;
using CyberCity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Implement
{
    public class CourseEnrollmentService: ICourseEnrollmentService
    {
        private readonly CourseEnrollmentRepo _courseEnrollmentRepo;
        private IMapper _mapper;
        public CourseEnrollmentService(CourseEnrollmentRepo courseEnrollmentRepo, IMapper mapper)
        {
            _courseEnrollmentRepo = courseEnrollmentRepo;
            _mapper = mapper;
        }

        public async Task<bool> EnrollAsync(Guid courseId, Guid userId)
        {
            var exists = await _courseEnrollmentRepo.GetByUserAndCourseAsync(userId, courseId);
            if (exists != null) return false;

            var enrollment = new CourseEnrollment
            {
                Uid = Guid.NewGuid(),
                CourseUid = courseId,
                UserUid = userId,
                EnrolledAt = DateTime.Now
            };

            return await _courseEnrollmentRepo.CreateAsync(enrollment) > 0;
        }

        public async Task<List<EnrollmentDto>> GetMyEnrollmentsAsync(Guid userId)
        {
            var result = await _courseEnrollmentRepo.GetAll()
                .Where(e => e.UserUid == userId)
                .Include(e => e.CourseU)
                .ToListAsync();

            return _mapper.Map<List<EnrollmentDto>>(result);
        }

        public async Task<List<CourseEnrollmentDto>> GetEnrollmentsByCourseAsync(Guid courseId)
        {
            var result = await _courseEnrollmentRepo.GetAll()
                .Where(e => e.CourseUid == courseId)
                .Include(e => e.UserU)
                .ToListAsync();

            return _mapper.Map<List<CourseEnrollmentDto>>(result);
        }
    }
}
