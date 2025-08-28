using CyberCity.Application.Interface;
using CyberCity.DTOs.UserAccount;
using Microsoft.AspNetCore.Mvc;
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
			if (request == null || string.IsNullOrWhiteSpace(request.UsernameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
			{
				return BadRequest("Invalid credentials");
			}

			var response = await _authService.LoginAsync(request);
			if (response == null)
			{
				return Unauthorized("Username or password is incorrect");
			}

			return Ok(response);
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
	}
}


