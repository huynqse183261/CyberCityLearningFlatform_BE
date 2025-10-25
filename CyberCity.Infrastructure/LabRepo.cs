using CyberCity.Doman.DBContext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;
using Microsoft.EntityFrameworkCore;

namespace CyberCity.Infrastructure
{
    public interface ILabRepository
    {
        Task<Lab> GetByIdAsync(Guid id);
        Task<List<Lab>> GetLabsByModuleIdAsync(Guid moduleId);
        Task<Lab> GetLabWithComponentsAsync(Guid labId);
        Task<List<LabComponent>> GetLabComponentsAsync(Guid labId);
    }

    public class LabRepo : GenericRepository<Lab>, ILabRepository
    {
        private readonly CyberCityLearningFlatFormDBContext _dbContext;

        public LabRepo(CyberCityLearningFlatFormDBContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<List<Lab>> GetLabsByModuleIdAsync(Guid moduleId)
        {
            return await _dbContext.Labs
                .Where(l => l.ModuleUid == moduleId.ToString())
                .OrderBy(l => l.OrderIndex)
                .ToListAsync();
        }

        public async Task<Lab> GetLabWithComponentsAsync(Guid labId)
        {
            return await _dbContext.Labs
                .Include(l => l.LabComponents)
                .FirstOrDefaultAsync(l => l.Uid == labId.ToString());
        }

        public async Task<List<LabComponent>> GetLabComponentsAsync(Guid labId)
        {
            return await _dbContext.LabComponents
                .Where(lc => lc.LabUid == labId.ToString())
                .OrderBy(lc => lc.OrderIndex)
                .ToListAsync();
        }
    }
}

