using System.Threading.Tasks;
using CyberCity.DTOs.UserAccount;

namespace CyberCity.Application.Interface
{
	public interface IAuthService
	{
		Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
		Task<LoginResponseDto?> RegisterAsync(RegisterRequestDto request);
	}
}


