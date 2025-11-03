using CyberCity.DTOs.Labs;

namespace CyberCity.Application.Interface
{
    public interface ILabService
    {
        Task<List<LabDto>> GetLabsByModuleIdAsync(string moduleId);
        Task<LabWithComponentsDto> GetLabByIdAsync(string labId);
        Task<StartLabResponseDto> StartLabAsync(string labId);
        Task CompleteLabAsync(string labId, string studentId);
    }
}

