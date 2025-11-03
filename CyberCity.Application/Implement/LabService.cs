using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.DTOs.Labs;
using CyberCity.Infrastructure;

namespace CyberCity.Application.Implement
{
    public class LabService : ILabService
    {
        private readonly ILabRepository _labRepo;
        private readonly IMapper _mapper;

        public LabService(ILabRepository labRepo, IMapper mapper)
        {
            _labRepo = labRepo;
            _mapper = mapper;
        }

        public async Task<List<LabDto>> GetLabsByModuleIdAsync(string moduleId)
        {
            var labs = await _labRepo.GetLabsByModuleIdAsync(moduleId);
            return _mapper.Map<List<LabDto>>(labs);
        }

        public async Task<LabWithComponentsDto> GetLabByIdAsync(string labId)
        {
            var lab = await _labRepo.GetLabWithComponentsAsync(labId);
            if (lab == null)
            {
                throw new Exception("Lab không tồn tại");
            }

            var components = await _labRepo.GetLabComponentsAsync(labId);

            return new LabWithComponentsDto
            {
                Lab = _mapper.Map<LabDto>(lab),
                Components = _mapper.Map<List<LabComponentDto>>(components)
            };
        }

        public async Task<StartLabResponseDto> StartLabAsync(string labId)
        {
            var lab = await _labRepo.GetByIdAsync(labId);
            if (lab == null)
            {
                throw new Exception("Lab không tồn tại");
            }

            var components = await _labRepo.GetLabComponentsAsync(labId);
            
            // Giả lập việc khởi động VMs (trong thực tế sẽ gọi API của hệ thống ảo hóa)
            var componentStatuses = components.Select(c => new LabComponentStatusDto
            {
                ComponentId = c.Uid,
                Status = "running",
                AccessUrl = $"http://lab-vm-{c.Uid}.cybercity.local"
            }).ToList();

            return new StartLabResponseDto
            {
                SessionId = Guid.NewGuid().ToString(),
                Components = componentStatuses
            };
        }

        public async Task CompleteLabAsync(string labId, string studentId)
        {
            var lab = await _labRepo.GetByIdAsync(labId);
            if (lab == null)
            {
                throw new Exception("Lab không tồn tại");
            }

            // Lưu tiến độ hoàn thành lab (có thể tạo bảng LabProgress nếu cần)
            // Hiện tại chỉ return thành công
            await Task.CompletedTask;
        }
    }
}

