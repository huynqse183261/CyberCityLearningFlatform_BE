using AutoMapper;
using CyberCity.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CyberCity.DTOs;
namespace CyberCity.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseEnrollmentController : ControllerBase
    {
        private readonly ICourseEnrollmentService _enrollmentService;
        private readonly IMapper _mapper;
        public CourseEnrollmentController(ICourseEnrollmentService enrollmentService, IMapper mapper)
        {
            _enrollmentService = enrollmentService;
            _mapper = mapper;
        }

        // POST api/courses/{id}/enroll
        [HttpPost("courses/{id}/enroll")]
        public async Task<IActionResult> Enroll(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst("uid")!.Value);
            var success = await _enrollmentService.EnrollAsync(id, userId);
            if (!success) return BadRequest("Already enrolled or failed");

            return Ok(new { Message = "Enrolled successfully" });
        }

        // GET api/enrollments/me
        [HttpGet("enrollments/me")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var userId = Guid.Parse(User.FindFirst("uid")!.Value);
            var result = await _enrollmentService.GetMyEnrollmentsAsync(userId);

            return Ok(result);
        }

        // GET api/courses/{id}/enrollments
        [HttpGet("courses/{id}/enrollments")]
        public async Task<IActionResult> GetCourseEnrollments(Guid id)
        {
            var result = await _enrollmentService.GetEnrollmentsByCourseAsync(id);
            return Ok(result);
        }
    }
}
