using System.Threading.Tasks;
using CyberCity.DTOs.UserAccount;

namespace CyberCity.Application.Interface
{
	public interface IAuthService
	{
		Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
		Task<LoginResponseDto?> RegisterAsync(RegisterRequestDto request);
		Task<LoginResponseDto?> GoogleLoginAsync(GoogleLoginRequestDto request);
		Task<bool> IsEmailTakenAsync(string email);
		Task<bool> IsUsernameTakenAsync(string username);
		Task<bool> SendResetCodeAsync(ForgotPasswordRequestDto request);
		Task<bool> VerifyResetCodeAsync(VerifyResetCodeDto request);
		Task<bool> ResetPasswordAsync(ResetPasswordDto request);
	}
}


