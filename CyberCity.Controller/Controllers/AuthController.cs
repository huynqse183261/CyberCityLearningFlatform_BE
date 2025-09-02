using CyberCity.Application.Interface;
using CyberCity.DTOs.UserAccount;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CyberCity.Controller.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
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
			var response = await _authService.RegisterAsync(request);
			if (response == null)
			{
				return BadRequest("Cannot register user. Username or Email may exist, or invalid input.");
			}
			return Ok(response);
		}

		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return Ok();
		}
	}
}


