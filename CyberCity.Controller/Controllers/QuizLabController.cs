using CyberCity.Application.Interface;
using CyberCity.DTOs.Quizzes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controller.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class QuizLabController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly ILabService _labService;

        public QuizLabController(IQuizService quizService, ILabService labService)
        {
            _quizService = quizService;
            _labService = labService;
        }

        private Guid GetCurrentUserId()
        {
            var uidClaim = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(uidClaim) || !Guid.TryParse(uidClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng");
            }
            return userId;
        }

        // ============ QUIZ APIS ============

        /// <summary>
        /// GET /api/quizzes/:quizId - Lấy thông tin quiz và câu hỏi
        /// </summary>
        [HttpGet("quizzes/{quizId:guid}")]
        public async Task<IActionResult> GetQuizById(Guid quizId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var quiz = await _quizService.GetQuizByIdAsync(quizId, userId);
                return Ok(new { success = true, data = quiz });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/quizzes/:quizId/submit - Nộp bài quiz
        /// </summary>
        [HttpPost("quizzes/{quizId:guid}/submit")]
        public async Task<IActionResult> SubmitQuiz(Guid quizId, [FromBody] SubmitQuizDto submitDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _quizService.SubmitQuizAsync(quizId, userId, submitDto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ============ LAB APIS ============

        /// <summary>
        /// GET /api/modules/:moduleId/labs - Lấy danh sách lab của module
        /// </summary>
        [HttpGet("modules/{moduleId:guid}/labs")]
        public async Task<IActionResult> GetLabsByModule(Guid moduleId)
        {
            try
            {
                var labs = await _labService.GetLabsByModuleIdAsync(moduleId);
                return Ok(new { success = true, data = labs });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/labs/:labId - Lấy chi tiết lab và các thành phần
        /// </summary>
        [HttpGet("labs/{labId:guid}")]
        public async Task<IActionResult> GetLabById(Guid labId)
        {
            try
            {
                var lab = await _labService.GetLabByIdAsync(labId);
                return Ok(new { success = true, data = lab });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/labs/:labId/start - Bắt đầu lab (khởi động VMs)
        /// </summary>
        [HttpPost("labs/{labId:guid}/start")]
        public async Task<IActionResult> StartLab(Guid labId)
        {
            try
            {
                var result = await _labService.StartLabAsync(labId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/labs/:labId/complete - Hoàn thành lab
        /// </summary>
        [HttpPost("labs/{labId:guid}/complete")]
        public async Task<IActionResult> CompleteLab(Guid labId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _labService.CompleteLabAsync(labId, userId);
                return Ok(new { success = true, data = new { labProgress = "completed" } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}

