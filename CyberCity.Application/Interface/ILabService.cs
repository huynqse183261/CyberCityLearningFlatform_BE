using CyberCity.DTOs.Labs;

namespace CyberCity.Application.Interface
{
    public interface ILabService
    {
        Task<List<LabDto>> GetLabsByModuleIdAsync(Guid moduleId);
        Task<LabWithComponentsDto> GetLabByIdAsync(Guid labId);
        Task<StartLabResponseDto> StartLabAsync(Guid labId);
        Task CompleteLabAsync(Guid labId, Guid studentId);
    }
}

