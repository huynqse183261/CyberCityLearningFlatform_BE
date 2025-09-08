using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Modules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CyberCity.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleService _moduleService;
        private readonly IMapper _mapper;
        public ModuleController(IModuleService moduleService, IMapper mapper)
        {
            _moduleService = moduleService;
            _mapper = mapper;
        }

        // GET: api/Module?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetModules([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _moduleService.GetModuleAsync(pageNumber, pageSize);
            return Ok(result);
        }

        // GET: api/Module/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetModuleById(Guid id)
        {
            var module = await _moduleService.GetByIdAsync(id);
            if (module == null)
                return NotFound();

            var dto = _mapper.Map<ModuleDetailDto>(module);
            return Ok(dto);
        }

        // POST: api/Module
        [HttpPost]
        public async Task<IActionResult> CreateModule([FromBody] ModuleCreateDto moduleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var module = _mapper.Map<Module>(moduleDto);
            var newId = await _moduleService.CreateAsync(module);
            return CreatedAtAction(nameof(GetModuleById), new { id = newId }, moduleDto);
        }

        // PUT: api/Module/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateModule(Guid id, [FromBody] ModuleUpdateDto moduleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var module = await _moduleService.GetByIdAsync(id);
            if (module == null)
                return NotFound();

            _mapper.Map(moduleDto, module);
            var updated = await _moduleService.UpdateAsync(module);
            if (!updated)
                return StatusCode(StatusCodes.Status500InternalServerError, "Update failed");

            return Ok("update thành công");
        }

        // DELETE: api/Module/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule(Guid id)
        {
            var deleted = await _moduleService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
