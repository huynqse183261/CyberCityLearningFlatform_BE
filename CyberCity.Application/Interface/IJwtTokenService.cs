using CyberCity.Doman.Models;

namespace CyberCity.Application.Interface
{
	public interface IJwtTokenService
	{
		string GenerateToken(User user);
	}
}


