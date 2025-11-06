using System.Threading.Tasks;
using CyberCity.DTOs.Subscriptions;

namespace CyberCity.Application.Interface
{
    public interface ISubscriptionService
    {
        Task<SubscriptionAccessDto> CheckUserSubscriptionAccessAsync(string userUid);
        Task<ModuleAccessDto> CheckModuleAccessAsync(string userUid, string courseUid, int moduleIndex);
    }
}
