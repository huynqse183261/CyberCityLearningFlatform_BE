using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Organizations;

namespace CyberCity_AutoMapper
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            // Organization mappings
            CreateMap<CreateOrganizationRequestDto, Organization>();
            CreateMap<UpdateOrganizationRequestDto, Organization>();
            CreateMap<Organization, OrganizationDTO>()
                .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.OrgMembers != null ? src.OrgMembers.Count : 0));

            // Organization Member mappings
            CreateMap<AddMemberRequestDto, OrgMember>();
            CreateMap<UpdateMemberRoleRequestDto, OrgMember>();
            CreateMap<OrgMember, OrganizationMemberDTO>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.UserU.FullName))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.UserU.Email))
                .ForMember(dest => dest.UserUsername, opt => opt.MapFrom(src => src.UserU.Username))
                .ForMember(dest => dest.UserImage, opt => opt.MapFrom(src => src.UserU.Image));
        }
    }
}
