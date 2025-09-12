using CyberCity.Application.Implement;
using CyberCity.Application.Interface;
using CyberCity.DTOs.Subtopics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controller.Controllers
{
    [Route("api/SubtopicProgresses")]
    [ApiController]
    public class SubtopicProgressController : ControllerBase
    {
        private readonly ISubtopicProgressService _service;
        public SubtopicProgressController(ISubtopicProgressService subtopicProgressService)
        {
            _service = subtopicProgressService;
        }
        [HttpGet]
        public async Task<IActionResult> GetsubtopicprogressAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _service.GetAllsubtopicProgressAsync(page, pageSize);
            return Ok(pagedResult);
        }
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudent(Guid studentId)
        {
            var result = await _service.GetByStudentAsync(studentId);
            return Ok(result);
        }

        [HttpGet("subtopic/{subtopicId}/student/{studentId}")]
        public async Task<IActionResult> GetBySubtopicAndStudent(Guid subtopicId, Guid studentId)
        {
            var result = await _service.GetBySubtopicAndStudentAsync(subtopicId, studentId);
            return Ok(result);
        }


        [HttpPost("subtopic/{subtopicId}/complete")]
        public async Task<IActionResult> MarkComplete(Guid subtopicId, [FromQuery] Guid studentId)
        {
            var result = await _service.MarkCompleteAsync(subtopicId, studentId);
            return Ok(result);
        }
    }
}
