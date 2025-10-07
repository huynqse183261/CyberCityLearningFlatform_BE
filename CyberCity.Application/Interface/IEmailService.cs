using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string htmlBody);
    }
}
