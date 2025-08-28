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
		}
	}
}


