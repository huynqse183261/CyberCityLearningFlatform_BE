using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CyberCity.Application.Implement
{
	public class JwtTokenService : IJwtTokenService
	{
		private readonly IConfiguration _configuration;

		public JwtTokenService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string GenerateToken(User user)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expires = DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpiresMinutes"] ?? "60"));

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Uid.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
				new Claim(JwtRegisteredClaimNames.UniqueName, user.Username ?? string.Empty),
				new Claim(ClaimTypes.Role, user.Role ?? "student")
			};

			var token = new JwtSecurityToken(
				audience: _configuration["Jwt:Audience"],
				issuer: _configuration["Jwt:Issuer"],
				claims: claims,
				expires: expires,
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}


