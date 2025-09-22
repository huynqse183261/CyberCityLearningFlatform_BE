using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.UserAccount;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CyberCity.Controller.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService; // Thêm CloudinaryService

        public UserController(IUserService userService, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _userService = userService;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
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

        //[Authorize(Roles = "admin")]
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

        [HttpPost("admin")]
        public async Task<ActionResult<UserAccountDTO>> CreateByAdmin([FromBody] CreateUserRequestDto request)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var user = _mapper.Map<User>(request);
            user.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            var created = await _userService.CreateAccountByAdmin(user);
            if (created <= 0) return BadRequest("Cannot create user");
            return Ok(_mapper.Map<UserAccountDTO>(user));
        }

        [HttpPut("{id}/password")]
        public async Task<ActionResult> UpdatePassword(Guid id, [FromBody] UpdatePasswordDto request)
        {
            if (id == Guid.Empty) return BadRequest("Invalid id");
            var ok = await _userService.UpdatePasswordAsync(id, request?.CurrentPassword, request?.NewPassword);
            if (!ok) return BadRequest("Invalid password or update failed");
            return NoContent();
        }

        //[Authorize(Roles = "admin")]
        [HttpPut("{id}/role")]
        public async Task<ActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleDto request)
        {
            if (id == Guid.Empty) return BadRequest("Invalid id");
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            user.Role = request?.Role;
            var updated = await _userService.UpdateAccount(user);
            if (updated <= 0) return BadRequest("Cannot update role");
            return NoContent();
        }

        //[Authorize(Roles = "admin")]
        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusDto request)
        {
            if (id == Guid.Empty) return BadRequest("Invalid id");
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            user.Status = request?.Status;
            var updated = await _userService.UpdateAccount(user);
            if (updated <= 0) return BadRequest("Cannot update status");
            return NoContent();
        }

        [HttpPut("{id}/avatar")]
        public async Task<ActionResult> UpdateAvatar(Guid id, [FromForm] UpdateAvatarDto dto)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid user id");

            if (dto?.Avatar == null || dto.Avatar.Length == 0)
                return BadRequest("Missing avatar file");

            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            // Upload ảnh lên Cloudinary
            var imageUrl = await _cloudinaryService.UploadImageAsync(dto.Avatar);
            if (string.IsNullOrEmpty(imageUrl))
                return BadRequest("Avatar upload failed");

            // Cập nhật avatar mới
            user.Image = imageUrl;
            var updated = await _userService.UpdateAccount(user);

            if (updated <= 0)
                return StatusCode(500, "Failed to update avatar");

            return Ok(new { avatarUrl = imageUrl });
        }


    }
}
