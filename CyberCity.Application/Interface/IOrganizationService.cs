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
        Task<Organization> GetOrganizationByIdAsync(string id);
        Task<Organization> CreateOrganizationAsync(CreateOrganizationRequestDto request);
        Task<Organization> UpdateOrganizationAsync(string id, UpdateOrganizationRequestDto request);
        Task<bool> DeleteOrganizationAsync(string id);

        // Organization Members operations
        Task<PagedResult<OrganizationMemberDTO>> GetOrganizationMembersAsync(string organizationId, int pageNumber, int pageSize);
        Task<OrganizationMemberDTO> AddMemberToOrganizationAsync(string organizationId, AddMemberRequestDto request);
        Task<OrganizationMemberDTO> UpdateMemberRoleAsync(string organizationId, string userId, UpdateMemberRoleRequestDto request);
        Task<bool> RemoveMemberFromOrganizationAsync(string organizationId, string userId);
        Task<bool> IsUserMemberOfOrganizationAsync(string organizationId, string userId);
        Task<string> GetUserRoleInOrganizationAsync(string organizationId, string userId);
        Task<int> GetOrganizationMemberCountAsync(string organizationId);
    }
}
