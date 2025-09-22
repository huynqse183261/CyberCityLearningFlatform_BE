using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Organizations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Controller.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;
        private readonly IMapper _mapper;

        public OrganizationController(IOrganizationService organizationService, IMapper mapper)
        {
            _organizationService = organizationService;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách tổ chức
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<OrganizationDTO>>> GetOrganizations(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] bool descending = true)
        {
            try
            {
                var result = await _organizationService.GetAllOrganizationsAsync(pageNumber, pageSize, descending);
                var organizationDtos = new List<OrganizationDTO>();
                
                foreach (var org in result.Items)
                {
                    var orgDto = _mapper.Map<OrganizationDTO>(org);
                    // Đảm bảo MemberCount được tính chính xác
                    orgDto.MemberCount = org.OrgMembers?.Count ?? 0;
                    organizationDtos.Add(orgDto);
                }
                
                var pagedResult = new PagedResult<OrganizationDTO>
                {
                    Items = organizationDtos,
                    TotalItems = result.TotalItems,
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize,
                    TotalPages = result.TotalPages
                };

                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách tổ chức", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy chi tiết tổ chức
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizationDTO>> GetOrganization(Guid id)
        {
            try
            {
                var organization = await _organizationService.GetOrganizationByIdAsync(id);
                if (organization == null)
                    return NotFound(new { message = "Tổ chức không tồn tại" });

                var organizationDto = _mapper.Map<OrganizationDTO>(organization);
                // Đảm bảo MemberCount được tính chính xác
                organizationDto.MemberCount = organization.OrgMembers?.Count ?? 0;
                return Ok(organizationDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin tổ chức", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo tổ chức mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<OrganizationDTO>> CreateOrganization([FromBody] CreateOrganizationRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var organization = await _organizationService.CreateOrganizationAsync(request);
                var organizationDto = _mapper.Map<OrganizationDTO>(organization);
                // Đảm bảo MemberCount được tính chính xác
                organizationDto.MemberCount = organization.OrgMembers?.Count ?? 0;
                
                return CreatedAtAction(nameof(GetOrganization), new { id = organization.Uid }, organizationDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo tổ chức", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật tổ chức
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<OrganizationDTO>> UpdateOrganization(Guid id, [FromBody] UpdateOrganizationRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var organization = await _organizationService.UpdateOrganizationAsync(id, request);
                var organizationDto = _mapper.Map<OrganizationDTO>(organization);
                // Đảm bảo MemberCount được tính chính xác
                organizationDto.MemberCount = organization.OrgMembers?.Count ?? 0;
                
                return Ok(organizationDto);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật tổ chức", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa tổ chức
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrganization(Guid id)
        {
            try
            {
                var result = await _organizationService.DeleteOrganizationAsync(id);
                if (!result)
                    return NotFound(new { message = "Tổ chức không tồn tại" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa tổ chức", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách thành viên của tổ chức
        /// </summary>
        [HttpGet("{id}/members")]
        public async Task<ActionResult<PagedResult<OrganizationMemberDTO>>> GetOrganizationMembers(
            Guid id,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _organizationService.GetOrganizationMembersAsync(id, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách thành viên", error = ex.Message });
            }
        }

        /// <summary>
        /// Thêm thành viên vào tổ chức
        /// </summary>
        [HttpPost("{id}/members")]
        public async Task<ActionResult<OrganizationMemberDTO>> AddMember(Guid id, [FromBody] AddMemberRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var member = await _organizationService.AddMemberToOrganizationAsync(id, request);
                return CreatedAtAction(nameof(GetOrganizationMembers), new { id = id }, member);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi thêm thành viên", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật vai trò thành viên
        /// </summary>
        [HttpPut("{id}/members/{userId}/role")]
        public async Task<ActionResult<OrganizationMemberDTO>> UpdateMemberRole(
            Guid id, 
            Guid userId, 
            [FromBody] UpdateMemberRoleRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var member = await _organizationService.UpdateMemberRoleAsync(id, userId, request);
                return Ok(member);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật vai trò thành viên", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa thành viên khỏi tổ chức
        /// </summary>
        [HttpDelete("{id}/members/{userId}")]
        public async Task<ActionResult> RemoveMember(Guid id, Guid userId)
        {
            try
            {
                var result = await _organizationService.RemoveMemberFromOrganizationAsync(id, userId);
                if (!result)
                    return NotFound(new { message = "Thành viên không tồn tại trong tổ chức" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa thành viên", error = ex.Message });
            }
        }

        /// <summary>
        /// Đếm số thành viên trong tổ chức
        /// </summary>
        [HttpGet("{id}/members/count")]
        public async Task<ActionResult<int>> GetOrganizationMemberCount(Guid id)
        {
            try
            {
                var count = await _organizationService.GetOrganizationMemberCountAsync(id);
                return Ok(new { memberCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi đếm số thành viên", error = ex.Message });
            }
        }
    }
}
