using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Lessons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        private readonly IMapper _mapper;
        public LessonController(ILessonService lessonService, IMapper mapper)
        {
            _lessonService = lessonService;
            _mapper = mapper;
        }
        // GET: api/Lesson?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetLessons([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _lessonService.GetLessonAsync(pageNumber, pageSize);
            return Ok(result);
        }
        // GET: api/Lesson/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLessonById(Guid id)
        {
            var lesson = await _lessonService.GetByIdAsync(id);
            if (lesson == null)
                return NotFound();
            var dto = _mapper.Map<LessonDetailDto>(lesson);
            return Ok(dto);
        }
        // POST: api/Lesson
        [HttpPost]
        public async Task<IActionResult> CreateLesson([FromBody] LessonCreateDto lessonDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var lesson = _mapper.Map<Lesson>(lessonDto);
            var newId = await _lessonService.CreateAsync(lesson);
            return CreatedAtAction(nameof(GetLessonById), new { id = newId }, lessonDto);
        }
        // PUT: api/Lesson/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLesson(Guid id, [FromBody] LessonUpdateDto lessonDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var existingLesson = await _lessonService.GetByIdAsync(id);
            if (existingLesson == null)
                return NotFound();
            _mapper.Map(lessonDto, existingLesson);
            var updated = await _lessonService.UpdateAsync(existingLesson);
            if (!updated)
                return StatusCode(500, "A problem happened while handling your request.");
            return Ok ("update thành công");
        }
        // DELETE: api/Lesson/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLesson(Guid id)
        {
            var existingLesson = await _lessonService.GetByIdAsync(id);
            if (existingLesson == null)
                return NotFound();
            var deleted = await _lessonService.DeleteAsync(id);
            if (!deleted)
                return StatusCode(500, "A problem happened while handling your request.");
            return Ok("Delete thành công");
        }

    }
}
