using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Organizations;
using CyberCity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Implement
{
    public class OrganizationService : IOrganizationService
    {
        private readonly OrganizationRepo _organizationRepo;
        private readonly OrgMemberRepo _orgMemberRepo;
        private readonly IMapper _mapper;

        public OrganizationService(OrganizationRepo organizationRepo, OrgMemberRepo orgMemberRepo, IMapper mapper)
        {
            _organizationRepo = organizationRepo;
            _orgMemberRepo = orgMemberRepo;
            _mapper = mapper;
        }

        public async Task<PagedResult<Organization>> GetAllOrganizationsAsync(int pageNumber, int pageSize, bool descending = true)
        {
            var query = _organizationRepo.GetAllAsync(descending)
                .Include(o => o.OrgMembers);
            var totalCount = await query.CountAsync();
            
            var organizations = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Organization>
            {
                Items = organizations,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<Organization> GetOrganizationByIdAsync(Guid id)
        {
            return await _organizationRepo.GetAllAsync()
                .Include(o => o.OrgMembers)
                .FirstOrDefaultAsync(o => o.Uid == id);
        }

        public async Task<Organization> CreateOrganizationAsync(CreateOrganizationRequestDto request)
        {
            var organization = _mapper.Map<Organization>(request);
            organization.Uid = Guid.NewGuid();
            organization.CreatedAt = DateTime.UtcNow;

            await _organizationRepo.CreateAsync(organization);
            
            // Load với OrgMembers để có MemberCount
            return await _organizationRepo.GetAllAsync()
                .Include(o => o.OrgMembers)
                .FirstOrDefaultAsync(o => o.Uid == organization.Uid);
        }

        public async Task<Organization> UpdateOrganizationAsync(Guid id, UpdateOrganizationRequestDto request)
        {
            var organization = await _organizationRepo.GetAllAsync()
                .Include(o => o.OrgMembers)
                .FirstOrDefaultAsync(o => o.Uid == id);
            if (organization == null)
                throw new ArgumentException("Tổ chức không tồn tại");

            _mapper.Map(request, organization);
            await _organizationRepo.UpdateAsync(organization);
            
            // Load lại với OrgMembers để có MemberCount
            return await _organizationRepo.GetAllAsync()
                .Include(o => o.OrgMembers)
                .FirstOrDefaultAsync(o => o.Uid == id);
        }

        public async Task<bool> DeleteOrganizationAsync(Guid id)
        {
            var organization = await _organizationRepo.GetByIdAsync(id);
            if (organization == null)
                return false;

            await _organizationRepo.RemoveAsync(organization);
            return true;
        }

        public async Task<PagedResult<OrganizationMemberDTO>> GetOrganizationMembersAsync(Guid organizationId, int pageNumber, int pageSize)
        {
            var query = _orgMemberRepo.GetAllAsync()
                .Where(om => om.OrgUid == organizationId)
                .Include(om => om.UserU);

            var totalCount = await query.CountAsync();

            var members = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(om => new OrganizationMemberDTO
                {
                    Uid = om.Uid,
                    OrgUid = om.OrgUid,
                    UserUid = om.UserUid,
                    MemberRole = om.MemberRole,
                    JoinedAt = om.JoinedAt,
                    UserFullName = om.UserU.FullName,
                    UserEmail = om.UserU.Email,
                    UserUsername = om.UserU.Username,
                    UserImage = om.UserU.Image
                })
                .ToListAsync();

            return new PagedResult<OrganizationMemberDTO>
            {
                Items = members,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<OrganizationMemberDTO> AddMemberToOrganizationAsync(Guid organizationId, AddMemberRequestDto request)
        {
            // Kiểm tra tổ chức có tồn tại không
            var organization = await _organizationRepo.GetByIdAsync(organizationId);
            if (organization == null)
                throw new ArgumentException("Tổ chức không tồn tại");

            // Kiểm tra user đã là thành viên chưa
            var existingMember = await _orgMemberRepo.GetAllAsync()
                .FirstOrDefaultAsync(om => om.OrgUid == organizationId && om.UserUid == request.UserUid);
            
            if (existingMember != null)
                throw new InvalidOperationException("User đã là thành viên của tổ chức này");

            var orgMember = new OrgMember
            {
                Uid = Guid.NewGuid(),
                OrgUid = organizationId,
                UserUid = request.UserUid,
                MemberRole = request.MemberRole,
                JoinedAt = DateTime.UtcNow
            };

            await _orgMemberRepo.CreateAsync(orgMember);

            // Lấy thông tin user để trả về
            var memberWithUser = await _orgMemberRepo.GetAllAsync()
                .Include(om => om.UserU)
                .FirstOrDefaultAsync(om => om.Uid == orgMember.Uid);

            if (memberWithUser == null || memberWithUser.UserU == null)
                throw new InvalidOperationException("Không tìm thấy thông tin user sau khi thêm thành viên. Hãy kiểm tra lại UserUid.");

            return new OrganizationMemberDTO
            {
                Uid = memberWithUser.Uid,
                OrgUid = memberWithUser.OrgUid,
                UserUid = memberWithUser.UserUid,
                MemberRole = memberWithUser.MemberRole,
                JoinedAt = memberWithUser.JoinedAt,
                UserFullName = memberWithUser.UserU.FullName,
                UserEmail = memberWithUser.UserU.Email,
                UserUsername = memberWithUser.UserU.Username,
                UserImage = memberWithUser.UserU.Image
            };
        }

        public async Task<OrganizationMemberDTO> UpdateMemberRoleAsync(Guid organizationId, Guid userId, UpdateMemberRoleRequestDto request)
        {
            var orgMember = await _orgMemberRepo.GetAllAsync()
                .Include(om => om.UserU)
                .FirstOrDefaultAsync(om => om.OrgUid == organizationId && om.UserUid == userId);

            if (orgMember == null)
                throw new ArgumentException("Thành viên không tồn tại trong tổ chức");

            orgMember.MemberRole = request.MemberRole;
            await _orgMemberRepo.UpdateAsync(orgMember);

            return new OrganizationMemberDTO
            {
                Uid = orgMember.Uid,
                OrgUid = orgMember.OrgUid,
                UserUid = orgMember.UserUid,
                MemberRole = orgMember.MemberRole,
                JoinedAt = orgMember.JoinedAt,
                UserFullName = orgMember.UserU.FullName,
                UserEmail = orgMember.UserU.Email,
                UserUsername = orgMember.UserU.Username,
                UserImage = orgMember.UserU.Image
            };
        }

        public async Task<bool> RemoveMemberFromOrganizationAsync(Guid organizationId, Guid userId)
        {
            var orgMember = await _orgMemberRepo.GetAllAsync()
                .FirstOrDefaultAsync(om => om.OrgUid == organizationId && om.UserUid == userId);

            if (orgMember == null)
                return false;

            await _orgMemberRepo.RemoveAsync(orgMember);
            return true;
        }

        public async Task<bool> IsUserMemberOfOrganizationAsync(Guid organizationId, Guid userId)
        {
            return await _orgMemberRepo.GetAllAsync()
                .AnyAsync(om => om.OrgUid == organizationId && om.UserUid == userId);
        }

        public async Task<string> GetUserRoleInOrganizationAsync(Guid organizationId, Guid userId)
        {
            var orgMember = await _orgMemberRepo.GetAllAsync()
                .FirstOrDefaultAsync(om => om.OrgUid == organizationId && om.UserUid == userId);

            return orgMember?.MemberRole ?? string.Empty;
        }

        public async Task<int> GetOrganizationMemberCountAsync(Guid organizationId)
        {
            return await _orgMemberRepo.GetAllAsync()
                .CountAsync(om => om.OrgUid == organizationId);
        }
    }
}
