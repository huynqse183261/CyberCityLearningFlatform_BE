using CyberCity.Application.Interface;
using Microsoft.AspNetCore.SignalR;
using CyberCity.Controller.Hubs;
using CyberCity.DTOs.Teacher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CyberCity.Controller.Controllers
{
    [ApiController]
    [Route("api/v1/teacher")]
    [Authorize(Roles = "teacher")]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;
        private readonly IHubContext<ChatHub> _hubContext;

        public TeacherController(ITeacherService teacherService, IHubContext<ChatHub> hubContext)
        {
            _teacherService = teacherService;
            _hubContext = hubContext;
        }

        private string GetTeacherUid()
        {
            var uidClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return uidClaim ?? string.Empty;
        }

        /// <summary>
        /// Lấy danh sách học viên của giáo viên
        /// </summary>
        [HttpGet("students")]
        public async Task<IActionResult> GetStudents(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? courseUid = null)
        {
            var teacherUid = GetTeacherUid();
            if (string.IsNullOrEmpty(teacherUid))
                return Unauthorized(new { success = false, message = "Không xác định được giáo viên" });

            var result = await _teacherService.GetStudentsAsync(teacherUid, page, limit, search, courseUid);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một học viên
        /// </summary>
        [HttpGet("students/{studentUid}")]
        public async Task<IActionResult> GetStudentDetail([FromRoute] string studentUid)
        {
            var teacherUid = GetTeacherUid();
            if (string.IsNullOrEmpty(teacherUid))
                return Unauthorized(new { success = false, message = "Không xác định được giáo viên" });

            var result = await _teacherService.GetStudentDetailAsync(teacherUid, studentUid);
            
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Thêm học viên vào khóa học
        /// </summary>
        [HttpPost("students/add")]
        public async Task<IActionResult> AddStudent([FromBody] AddStudentRequest request)
        {
            var teacherUid = GetTeacherUid();
            if (string.IsNullOrEmpty(teacherUid))
                return Unauthorized(new { success = false, message = "Không xác định được giáo viên" });

            if (string.IsNullOrEmpty(request.StudentUid) || string.IsNullOrEmpty(request.CourseUid))
                return BadRequest(new { success = false, message = "StudentUid và CourseUid là bắt buộc" });

            var result = await _teacherService.AddStudentAsync(teacherUid, request);
            return Ok(result);
        }

        /// <summary>
        /// Xóa học viên khỏi khóa học
        /// </summary>
        [HttpDelete("students/{studentUid}")]
        public async Task<IActionResult> RemoveStudent([FromRoute] string studentUid, [FromQuery] string courseUid)
        {
            var teacherUid = GetTeacherUid();
            if (string.IsNullOrEmpty(teacherUid))
                return Unauthorized(new { success = false, message = "Không xác định được giáo viên" });

            if (string.IsNullOrEmpty(courseUid))
                return BadRequest(new { success = false, message = "CourseUid là bắt buộc" });

            var result = await _teacherService.RemoveStudentAsync(teacherUid, studentUid, courseUid);
            
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Xem tiến độ học tập của học viên
        /// </summary>
        [HttpGet("students/{studentUid}/progress")]
        public async Task<IActionResult> GetStudentProgress(
            [FromRoute] string studentUid,
            [FromQuery] string? courseUid = null)
        {
            var teacherUid = GetTeacherUid();
            if (string.IsNullOrEmpty(teacherUid))
                return Unauthorized(new { success = false, message = "Không xác định được giáo viên" });

            var result = await _teacherService.GetStudentProgressAsync(teacherUid, studentUid, courseUid);
            
            if (!result.Success)
                return Forbid();

            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách hội thoại của giáo viên
        /// </summary>
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            var teacherUid = GetTeacherUid();
            if (string.IsNullOrEmpty(teacherUid))
                return Unauthorized(new { success = false, message = "Không xác định được giáo viên" });

            var result = await _teacherService.GetConversationsAsync(teacherUid, page, limit);
            return Ok(result);
        }

        /// <summary>
        /// Tạo hội thoại mới với học viên
        /// </summary>
        [HttpPost("conversations")]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest request)
        {
            var teacherUid = GetTeacherUid();
            if (string.IsNullOrEmpty(teacherUid))
                return Unauthorized(new { success = false, message = "Không xác định được giáo viên" });

            if (string.IsNullOrEmpty(request.StudentUid))
                return BadRequest(new { success = false, message = "StudentUid là bắt buộc" });

            var result = await _teacherService.CreateConversationAsync(teacherUid, request);
            return Ok(result);
        }

        /// <summary>
        /// Lấy tin nhắn trong hội thoại
        /// </summary>
        [HttpGet("conversations/{conversationUid}/messages")]
        public async Task<IActionResult> GetConversationMessages(
            [FromRoute] string conversationUid,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20,
            [FromQuery] DateTime? before = null)
        {
            var teacherUid = GetTeacherUid();
            if (string.IsNullOrEmpty(teacherUid))
                return Unauthorized(new { success = false, message = "Không xác định được giáo viên" });

            var result = await _teacherService.GetConversationMessagesAsync(
                teacherUid,
                conversationUid,
                page,
                limit,
                before
            );

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Gửi tin nhắn trong hội thoại
        /// </summary>
        [HttpPost("conversations/{conversationUid}/messages")]
        public async Task<IActionResult> SendMessage(
            [FromRoute] string conversationUid,
            [FromBody] SendMessageRequest request)
        {
            var teacherUid = GetTeacherUid();
            if (string.IsNullOrEmpty(teacherUid))
                return Unauthorized(new { success = false, message = "Không xác định được giáo viên" });

            var result = await _teacherService.SendMessageAsync(teacherUid, conversationUid, request);
            
            if (!result.Success)
                return BadRequest(result);

            // Broadcast real-time to conversation group
            await _hubContext.Clients.Group($"conversation_{conversationUid}").SendAsync("ReceiveMessage", result.Data);

            return Ok(result);
        }

        /// <summary>
        /// Đánh dấu tin nhắn đã đọc
        /// </summary>
        [HttpPatch("conversations/{conversationUid}/read")]
        public async Task<IActionResult> MarkAsRead([FromRoute] string conversationUid)
        {
            var teacherUid = GetTeacherUid();
            if (string.IsNullOrEmpty(teacherUid))
                return Unauthorized(new { success = false, message = "Không xác định được giáo viên" });

            var result = await _teacherService.MarkAsReadAsync(teacherUid, conversationUid);
            
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Lấy thống kê dashboard của giáo viên
        /// </summary>
        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var teacherUid = GetTeacherUid();
            if (string.IsNullOrEmpty(teacherUid))
                return Unauthorized(new { success = false, message = "Không xác định được giáo viên" });

            var result = await _teacherService.GetDashboardStatsAsync(teacherUid);
            return Ok(result);
        }
    }
}
