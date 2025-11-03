using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Subtopics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controller.Controllers
{
    [Route("api/subtopics")]
    [ApiController]
    public class SubtopicController : ControllerBase
    {
        private readonly ISubtopicService _subtopicService;
        private IMapper _mapper;
        public SubtopicController(ISubtopicService subtopicService, IMapper mapper)
        {
            _subtopicService = subtopicService;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> CreateSubtopic([FromBody] SubtopicCreateDto subtopicCreateDto)
        {
            var subtopic = _mapper.Map<Subtopic>(subtopicCreateDto);
            var result = await _subtopicService.CreateAsync(subtopic);
            if (string.IsNullOrEmpty(result))
            {
                return BadRequest("Failed to create subtopic");
            }
            return Ok(result);
        }

        [HttpGet("{uid}")]
        public async Task<IActionResult> GetSubtopicById(string uid)
        {
            var subtopic = await _subtopicService.GetByIdAsync(uid);
            if (subtopic == null)
            {
                return NotFound();
            }
            var subtopicDetailDto = _mapper.Map<SubtopicDetailDto>(subtopic);
            return Ok(subtopicDetailDto);
        }
        [HttpGet]
        public async Task<IActionResult> GetSubtopics([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _subtopicService.GetSubtopicAsync(page, pageSize);
            return Ok(pagedResult);
        }
        [HttpDelete("{uid}")]
        public async Task<IActionResult> DeleteSubtopic(string uid)
        {
            var result = await _subtopicService.DeleteAsync(uid);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        [HttpPut("{uid}")]
        public async Task<IActionResult> UpdateSubtopic(string uid, [FromBody] SubtopicUpdateDto subtopicUpdateDto)
        {
            var existingSubtopic = await _subtopicService.GetByIdAsync(uid);
            if (existingSubtopic == null)
            {
                return NotFound();
            }
            _mapper.Map(subtopicUpdateDto, existingSubtopic);
            var result = await _subtopicService.UpdateAsync(existingSubtopic);
            if (!result)
            {
                return BadRequest("Failed to update subtopic");
            }
            return NoContent();
        }

    }
}
