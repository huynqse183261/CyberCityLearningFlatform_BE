using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface IOrganizationService
    {
        // Organization CRUD operations
        Task<PagedResult<Organization>> GetAllOrganizationsAsync(int pageNumber, int pageSize, bool descending = true);
        Task<Organization> GetOrganizationByIdAsync(Guid id);
        Task<Organization> CreateOrganizationAsync(CreateOrganizationRequestDto request);
        Task<Organization> UpdateOrganizationAsync(Guid id, UpdateOrganizationRequestDto request);
        Task<bool> DeleteOrganizationAsync(Guid id);

        // Organization Members operations
        Task<PagedResult<OrganizationMemberDTO>> GetOrganizationMembersAsync(Guid organizationId, int pageNumber, int pageSize);
        Task<OrganizationMemberDTO> AddMemberToOrganizationAsync(Guid organizationId, AddMemberRequestDto request);
        Task<OrganizationMemberDTO> UpdateMemberRoleAsync(Guid organizationId, Guid userId, UpdateMemberRoleRequestDto request);
        Task<bool> RemoveMemberFromOrganizationAsync(Guid organizationId, Guid userId);
        Task<bool> IsUserMemberOfOrganizationAsync(Guid organizationId, Guid userId);
        Task<string> GetUserRoleInOrganizationAsync(Guid organizationId, Guid userId);
        Task<int> GetOrganizationMemberCountAsync(Guid organizationId);
    }
}
