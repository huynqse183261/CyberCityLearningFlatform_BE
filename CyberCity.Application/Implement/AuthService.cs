using System.Threading.Tasks;
using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.DTOs.UserAccount;
using CyberCity.Infrastructure;

namespace CyberCity.Application.Implement
{
	public class AuthService : IAuthService
	{
		private readonly UserRepo _userRepo;
		private readonly IMapper _mapper;
		private readonly IJwtTokenService _jwtTokenService;

		public AuthService(UserRepo userRepo, IMapper mapper, IJwtTokenService jwtTokenService)
		{
			_userRepo = userRepo;
			_mapper = mapper;
			_jwtTokenService = jwtTokenService;
		}

		public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
			if (request == null || string.IsNullOrWhiteSpace(request.UsernameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
			{
				return null;
			}

			var user = await _userRepo.GetByUsernameOrEmailAsync(request.UsernameOrEmail);
			if (user == null)
			{
				return null;
			}

			var isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
			if (!isValid)
			{
				return null;
			}

            var response = _mapper.Map<LoginResponseDto>(user);
            response.Token = _jwtTokenService.GenerateToken(user);
            return response;
        }

		public async Task<LoginResponseDto?> RegisterAsync(RegisterRequestDto request)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.FullName))
			{
				return null;
			}

			var existed = await _userRepo.GetByUsernameOrEmailAsync(request.Username) ?? await _userRepo.GetByUsernameOrEmailAsync(request.Email);
			if (existed != null)
			{
				return null;
			}

			var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
			var newUser = new CyberCity.Doman.Models.User
			{
				Uid = System.Guid.NewGuid(),
				Email = request.Email.Trim(),
				Username = request.Username.Trim(),
				PasswordHash = hash,
				FullName = request.FullName.Trim(),
				Role = string.IsNullOrWhiteSpace(request.Role) ? "student" : request.Role.Trim(),
				CreatedAt = System.DateTime.Now
			};

			await _userRepo.CreateAsync(newUser);
            var created = _mapper.Map<LoginResponseDto>(newUser);
            created.Token = _jwtTokenService.GenerateToken(newUser);
            return created;
		}

    }
}


