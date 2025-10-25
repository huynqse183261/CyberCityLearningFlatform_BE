using System.Threading.Tasks;
using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.DTOs.UserAccount;
using CyberCity.Infrastructure;
using Google.Apis.Auth;
using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;

namespace CyberCity.Application.Implement
{
	public class AuthService : IAuthService
	{
		private readonly UserRepo _userRepo;
		private readonly IMapper _mapper;
		private readonly IJwtTokenService _jwtTokenService;
	        private static readonly ConcurrentDictionary<string, (string Code, DateTime Expires)> _resetCodes = new();
	        private readonly IEmailService _emailService;
	        private readonly IConfiguration _config;

			public AuthService(UserRepo userRepo, IMapper mapper, IJwtTokenService jwtTokenService, IEmailService emailService, IConfiguration config)
		{
			_userRepo = userRepo;
			_mapper = mapper;
			_jwtTokenService = jwtTokenService;
	            _emailService = emailService;
	            _config = config;
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

			// Normalize input
			var email = request.Email.Trim();
			var username = request.Username.Trim();

			// Check duplicates explicitly
			var emailExists = await _userRepo.GetByEmailAsync(email);
			if (emailExists != null)
			{
				return null;
			}

			var usernameExists = await _userRepo.GetByUsernameAsync(username);
			if (usernameExists != null)
			{
				return null;
			}

			var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
			var newUser = new CyberCity.Doman.Models.User
			{
				Uid = System.Guid.NewGuid().ToString(),
				Email = email,
				Username = username,
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

		public async Task<LoginResponseDto?> GoogleLoginAsync(GoogleLoginRequestDto request)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.IdToken))
			{
				return null;
			}

			GoogleJsonWebSignature.Payload payload;
			try
			{
				var clientId = _config["Google:ClientId"];
				var settings = new GoogleJsonWebSignature.ValidationSettings();
				if (!string.IsNullOrWhiteSpace(clientId))
				{
					settings.Audience = new[] { clientId };
				}
				payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
			}
			catch
			{
				return null;
			}

			// Upsert user by email
			var email = payload.Email?.Trim();
			if (string.IsNullOrWhiteSpace(email)) return null;
			// If email not verified, reject optionally (check for explicit false)
			if (payload.EmailVerified == false)
			{
				return null;
			}
			var existing = await _userRepo.GetByEmailAsync(email);
			if (existing == null)
			{
				var newUser = new CyberCity.Doman.Models.User
				{
					Uid = System.Guid.NewGuid().ToString(),
					Email = email,
					Username = (payload.Name ?? email).Trim(),
					PasswordHash = string.Empty,
					FullName = payload.Name,
					Role = "student",
					Image = payload.Picture,
					CreatedAt = System.DateTime.Now
				};
				await _userRepo.CreateAsync(newUser);
				existing = newUser;
			}

			var response = _mapper.Map<LoginResponseDto>(existing);
			response.Token = _jwtTokenService.GenerateToken(existing);
			return response;
		}

		public async Task<bool> SendResetCodeAsync(ForgotPasswordRequestDto request)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.Email)) return false;
			var email = request.Email.Trim();
			var user = await _userRepo.GetByEmailAsync(email);
			if (user == null) return false;

			var rng = new Random();
			var code = rng.Next(0, 999999).ToString("D6");
			var expires = DateTime.Now.AddMinutes(10);
			_resetCodes[email] = (code, expires);

			var subject = "Mã xác nhận đặt lại mật khẩu";
			var body = $"<p>Xin chào {user.FullName ?? user.Username},</p><p>Mã xác nhận của bạn là: <b>{code}</b></p><p>Mã có hiệu lực đến {expires:HH:mm dd/MM/yyyy} (UTC).</p>";
			await _emailService.SendAsync(email, subject, body);
			return true;
		}

		public Task<bool> VerifyResetCodeAsync(VerifyResetCodeDto request)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Code)) return Task.FromResult(false);
			var email = request.Email.Trim();
			if (!_resetCodes.TryGetValue(email, out var entry)) return Task.FromResult(false);
			if (entry.Expires < DateTime.Now) { _resetCodes.TryRemove(email, out _); return Task.FromResult(false); }
			var ok = string.Equals(entry.Code, request.Code.Trim(), StringComparison.Ordinal);
			return Task.FromResult(ok);
		}

		public async Task<bool> ResetPasswordAsync(ResetPasswordDto request)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.NewPassword)) return false;
			var email = request.Email.Trim();
			if (!_resetCodes.TryGetValue(email, out var entry)) return false;
			if (entry.Expires < DateTime.Now) { _resetCodes.TryRemove(email, out _); return false; }
			if (!string.Equals(entry.Code, request.Code.Trim(), StringComparison.Ordinal)) return false;

			var user = await _userRepo.GetByEmailAsync(email);
			if (user == null) return false;
			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
			await _userRepo.UpdateAsync(user);
			_resetCodes.TryRemove(email, out _);
			// notify user
			await _emailService.SendAsync(email, "Đặt lại mật khẩu thành công", $"<p>Xin chào {user.FullName ?? user.Username},</p><p>Bạn đã đặt lại mật khẩu thành công.</p>");
			return true;
		}
		public async Task<bool> IsEmailTakenAsync(string email)
		{
			if (string.IsNullOrWhiteSpace(email)) return false;
			var existed = await _userRepo.GetByEmailAsync(email.Trim());
			return existed != null;
		}

		public async Task<bool> IsUsernameTakenAsync(string username)
		{
			if (string.IsNullOrWhiteSpace(username)) return false;
			var existed = await _userRepo.GetByUsernameAsync(username.Trim());
			return existed != null;
		}

    }
}


