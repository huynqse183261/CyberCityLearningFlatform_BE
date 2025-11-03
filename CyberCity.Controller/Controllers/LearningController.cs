using CyberCity.Application.Interface;
using CyberCity.DTOs.Answers;
using CyberCity.DTOs.Enrollments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CyberCity.Controller.Controllers
{
    [ApiController]
    [Route("api/learning")]
    [Authorize]
    public class LearningController : ControllerBase
    {
        private readonly ILearningService _learningService;
        private readonly ICourseEnrollmentService _enrollmentService;

        public LearningController(ILearningService learningService, ICourseEnrollmentService enrollmentService)
        {
            _learningService = learningService;
            _enrollmentService = enrollmentService;
        }

        private string GetCurrentUserId()
        {
            var uidClaim = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(uidClaim))
            {
                throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng");
            }
            return uidClaim;
        }

        // ============ COURSE APIS ============

        /// <summary>
        /// GET /api/learning/courses - Lấy danh sách tất cả khóa học
        /// </summary>
        [HttpGet("courses")]
        public async Task<IActionResult> GetAllCourses()
        {
            try
            {
                var userId = GetCurrentUserId();
                var courses = await _learningService.GetAllCoursesAsync(userId);
                return Ok(new { success = true, data = courses });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/courses/:courseId - Lấy chi tiết một khóa học
        /// </summary>
        [HttpGet("courses/{courseId}")]
        public async Task<IActionResult> GetCourseDetail(string courseId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var courseDetail = await _learningService.GetCourseDetailAsync(courseId, userId);
                return Ok(new { success = true, data = courseDetail });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/courses/:courseId/enroll - Đăng ký khóa học
        /// </summary>
        [HttpPost("courses/{courseId}/enroll")]
        public async Task<IActionResult> EnrollCourse(string courseId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _enrollmentService.EnrollAsync(courseId, userId);
                return Ok(new { success = result, message = result ? "Enrolled successfully" : "Enrollment failed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ============ MODULE APIS ============

        /// <summary>
        /// GET /api/courses/:courseId/modules - Lấy danh sách module của khóa học
        /// </summary>
        [HttpGet("courses/{courseId}/modules")]
        public async Task<IActionResult> GetModulesByCourse(string courseId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var courseDetail = await _learningService.GetCourseDetailAsync(courseId, userId);
                return Ok(new { success = true, data = courseDetail.Modules });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/modules/:moduleId - Lấy chi tiết module kèm lessons và labs
        /// </summary>
        [HttpGet("modules/{moduleId}")]
        public async Task<IActionResult> GetModuleDetail(string moduleId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var moduleDetail = await _learningService.GetModuleDetailAsync(moduleId, userId);
                return Ok(new { success = true, data = moduleDetail });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ============ LESSON APIS ============

        /// <summary>
        /// GET /api/lessons/:lessonId - Lấy chi tiết bài học kèm topics và subtopics
        /// </summary>
        [HttpGet("lessons/{lessonId}")]
        [HttpGet("lessons/{lessonId}/content")]
        public async Task<IActionResult> GetLessonContent(string lessonId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var learningContent = await _learningService.GetLessonContentAsync(lessonId, userId);
                return Ok(new { success = true, data = learningContent });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ============ TOPIC & SUBTOPIC APIS ============

        /// <summary>
        /// POST /api/subtopics/:subtopicId/submit-answer - Nộp câu trả lời cho subtopic
        /// </summary>
        [HttpPost("subtopics/{subtopicId}/submit-answer")]
        public async Task<IActionResult> SubmitSubtopicAnswer(string subtopicId, [FromBody] SubmitAnswerDto submitDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _learningService.SubmitSubtopicAnswerAsync(subtopicId, userId, submitDto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/subtopics/:subtopicId/complete - Đánh dấu subtopic đã hoàn thành
        /// </summary>
        [HttpPost("subtopics/{subtopicId}/complete")]
        public async Task<IActionResult> CompleteSubtopic(string subtopicId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var progress = await _learningService.CompleteSubtopicAsync(subtopicId, userId);
                return Ok(new { success = true, data = progress });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ============ PROGRESS APIS ============

        /// <summary>
        /// GET /api/students/progress - Lấy tổng quan tiến độ của học viên
        /// </summary>
        [HttpGet("students/progress")]
        public async Task<IActionResult> GetStudentProgress()
        {
            try
            {
                var userId = GetCurrentUserId();
                var progress = await _learningService.GetStudentProgressAsync(userId);
                return Ok(new { success = true, data = progress });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/students/progress/course/:courseId - Lấy tiến độ chi tiết của một khóa học
        /// </summary>
        [HttpGet("students/progress/course/{courseId}")]
        public async Task<IActionResult> GetCourseProgress(string courseId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var progress = await _learningService.GetCourseProgressDetailAsync(courseId, userId);
                return Ok(new { success = true, data = progress });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}

