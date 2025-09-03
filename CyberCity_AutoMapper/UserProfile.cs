using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.UserAccount;

namespace CyberCity_AutoMapper
{
	public class UserProfile : Profile
	{
		public UserProfile()
		{
			CreateMap<User, LoginResponseDto>();
			CreateMap<User, UserAccountDTO>();
			CreateMap<CreateUserRequestDto, User>()
				.ForMember(d => d.PasswordHash, o => o.MapFrom(s => s.Password))
				.ForMember(d => d.CreatedAt, o => o.MapFrom(_ => DateTime.UtcNow));
			CreateMap<UpdateUserRequestDto, User>()
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
		}
	}
}


