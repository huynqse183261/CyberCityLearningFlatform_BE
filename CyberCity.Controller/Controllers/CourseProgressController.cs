using CyberCity.Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseProgressController : ControllerBase
    {
        private readonly ICourseProgressService _courseProgressService;
        public CourseProgressController(ICourseProgressService courseProgressService)
        {
            _courseProgressService = courseProgressService;
        }
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProgress([FromQuery] Guid courseId, [FromQuery] Guid studentId)
        {
            var progress = await _courseProgressService.GetMyProgressAsync(courseId, studentId);
            if (progress == null)
            {
                return NotFound();
            }
            return Ok(progress);
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetCourseProgressOverview([FromQuery] Guid courseId)
        {
            var overview = await _courseProgressService.GetCourseProgressOverviewAsync(courseId);
            return Ok(overview);
        }
    }
}
