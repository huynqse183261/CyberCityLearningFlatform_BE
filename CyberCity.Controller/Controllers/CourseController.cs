using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Courses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CyberCity.Controller.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;

        public CoursesController(ICourseService courseService, IMapper mapper)
        {
            _courseService = courseService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string level = null, [FromQuery] bool descending = true)
        {
            var result = await _courseService.GetCoursesAsync(pageNumber, pageSize, level, descending);
            var mapped = new PagedResult<CourseListItemResponse>
            {
                Items = result.Items.Select(c => _mapper.Map<CourseListItemResponse>(c)).ToList(),
                TotalItems = result.TotalItems,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
            return Ok(mapped);
        }

        [HttpGet("outline")]
        public async Task<IActionResult> GetAllOutline([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _courseService.GetAllOutline(page, pageSize);
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null) return NotFound();
            return Ok(_mapper.Map<CourseDetailResponse>(course));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourseCreateUpdateRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(ClaimTypes.Name);
            if (userIdClaim == null) return Unauthorized();
            if (!Guid.TryParse(userIdClaim.Value, out var creatorId)) return Unauthorized();

            var entity = _mapper.Map<Course>(request);
            entity.CreatedBy = creatorId.ToString();

            var uid = await _courseService.CreateAsync(entity);
            if (uid == Guid.Empty) return BadRequest("Cannot create course");
            var created = await _courseService.GetByIdAsync(uid);
            return CreatedAtAction(nameof(GetById), new { id = uid }, _mapper.Map<CourseDetailResponse>(created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CourseCreateUpdateRequest request)
        {
            var existing = await _courseService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _mapper.Map(request, existing);
            var ok = await _courseService.UpdateAsync(existing);
            if (!ok) return BadRequest("Cannot update course");

            var updated = await _courseService.GetByIdAsync(id);
            return Ok(_mapper.Map<CourseDetailResponse>(updated));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _courseService.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

    }
}