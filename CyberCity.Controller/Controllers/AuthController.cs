using CyberCity.Application.Interface;
using CyberCity.DTOs.UserAccount;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using CyberCity.DTOs.UserAccount;

namespace CyberCity.Controller.Controllers
{
	[ApiController]
	[Route("auth")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
		{
			var result = await _authService.LoginAsync(request);
			if (result == null)
				return Unauthorized();

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, result.Uid.ToString()),
				new Claim(ClaimTypes.Name, result.Username),
				new Claim(ClaimTypes.Email, result.Email),
				new Claim(ClaimTypes.Role, result.Role)
			};

			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var authProperties = new AuthenticationProperties
			{
				IsPersistent = true,
				ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
			};

			await HttpContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				new ClaimsPrincipal(claimsIdentity),
				authProperties);

			return Ok(result);
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
		{
			if (request == null)
				return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ.", data = (object?)null });

			if (await _authService.IsEmailTakenAsync(request.Email))
				return BadRequest(new { success = false, message = "Email đã tồn tại.", data = (object?)null });

			if (await _authService.IsUsernameTakenAsync(request.Username))
				return BadRequest(new { success = false, message = "Username đã tồn tại.", data = (object?)null });

			var response = await _authService.RegisterAsync(request);
			if (response == null)
				return BadRequest(new { success = false, message = "Không thể đăng ký. Dữ liệu không hợp lệ.", data = (object?)null });

			return Ok(new
			{
				success = true,
				message = "Đăng ký thành công",
				data = response
			});
		}

		[HttpPost("google-login")]
		public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto request)
		{
			var result = await _authService.GoogleLoginAsync(request);
			if (result == null)
				return Unauthorized();

			return Ok(new
			{
				success = true,
				message = "Đăng nhập Google thành công",
				data = result
			});
		}

		[HttpPost("forgot-password/send-code")]
		public async Task<IActionResult> SendResetCode([FromBody] ForgotPasswordRequestDto request)
		{
			var ok = await _authService.SendResetCodeAsync(request);
			if (!ok) return BadRequest(new { success = false, message = "Không tìm thấy email hoặc không thể gửi mã." });
			return Ok(new { success = true, message = "Đã gửi mã xác nhận 6 số tới email." });
		}

		[HttpPost("forgot-password/verify-code")]
		public async Task<IActionResult> VerifyCode([FromBody] VerifyResetCodeDto request)
		{
			var ok = await _authService.VerifyResetCodeAsync(request);
			if (!ok) return BadRequest(new { success = false, message = "Mã không hợp lệ hoặc đã hết hạn." });
			return Ok(new { success = true, message = "Mã hợp lệ." });
		}

		[HttpPost("forgot-password/reset")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
		{
			var ok = await _authService.ResetPasswordAsync(request);
			if (!ok) return BadRequest(new { success = false, message = "Không thể đặt lại mật khẩu. Kiểm tra email/mã hoặc mã đã hết hạn." });
			return Ok(new { success = true, message = "Đặt lại mật khẩu thành công." });
		}

		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return Ok();
		}
	}
}


