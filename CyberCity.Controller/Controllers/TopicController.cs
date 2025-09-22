using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Topics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controller.Controllers
{
    [Route("api/topics")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;
        private readonly IMapper _mapper;
        public TopicController(ITopicService topicService, IMapper mapper)
        {
            _topicService = topicService;
            _mapper = mapper;
        }
        // GET: api/Topic?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetTopics([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _topicService.GetTopicAsync(pageNumber, pageSize);
            return Ok(result);
        }
        // GET: api/Topic/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopicById(Guid id)
        {
            var topic = await _topicService.GetByIdAsync(id);
            if (topic == null)
                return NotFound();
            var dto = _mapper.Map<TopicDetailDto>(topic);
            return Ok(dto);
        }
        // POST: api/Topic
        [HttpPost]
        public async Task<IActionResult> CreateTopic([FromBody] TopicCreateDto topicDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var topic = _mapper.Map<Topic>(topicDto);
            var newId = await _topicService.CreateAsync(topic);
            return CreatedAtAction(nameof(GetTopicById), new { id = newId }, topicDto);
        }
        // PUT: api/Topic/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTopic(Guid id, [FromBody] TopicUpdateDto topicDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var existingTopic = await _topicService.GetByIdAsync(id);
            if (existingTopic == null)
                return NotFound();
            _mapper.Map(topicDto, existingTopic);
            var updated = await _topicService.UpdateAsync(existingTopic);
            if (!updated)
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating topic");
            return NoContent();
        }
        // DELETE: api/Topic/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(Guid id)
        {
            var existingTopic = await _topicService.GetByIdAsync(id);
            if (existingTopic == null)
                return NotFound();
            var deleted = await _topicService.DeleteAsync(id);
            if (!deleted)
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting topic");
            return NoContent();
        }
    }
}
