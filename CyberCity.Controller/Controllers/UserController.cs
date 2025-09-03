using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.UserAccount;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System;
using System.Threading.Tasks;

namespace CyberCity.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<UserAccountDTO>>> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, [FromQuery] bool descending = true)
        {
            var result = await _userService.GetAllAccounts(pageNumber, pageSize, descending);
            var dto = new PagedResult<UserAccountDTO>
            {
                Items = result.Items.ConvertAll(u => _mapper.Map<UserAccountDTO>(u)),
                TotalItems = result.TotalItems,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
            return Ok(dto);
        }

        [HttpGet("by-username-or-email")]
        public async Task<ActionResult<UserAccountDTO>> GetByUsernameOrEmail([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return BadRequest("Missing query parameter q");
            var user = await _userService.GetUserAccountByNameOrEmailAsync(q);
            if (user == null) return NotFound();
            return Ok(_mapper.Map<UserAccountDTO>(user));
        }

        [HttpPost]
        public async Task<ActionResult<UserAccountDTO>> Create([FromBody] CreateUserRequestDto request)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var user = _mapper.Map<User>(request);
            user.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            user.Role = "student"; 
            var created = await _userService.CreateAccount(user);
            if (created <= 0) return BadRequest("Cannot create user");
            return Ok(_mapper.Map<UserAccountDTO>(user));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateUserRequestDto request)
        {
            if (id == Guid.Empty) return BadRequest("Invalid id");
            var existing = await _userService.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(request, existing);
            var updated = await _userService.UpdateAccount(existing);
            if (updated <= 0) return BadRequest("Cannot update user");
            return NoContent();
        }
    }
}
