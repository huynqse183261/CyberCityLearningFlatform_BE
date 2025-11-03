using CyberCity.Doman.DBContext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;
using Microsoft.EntityFrameworkCore;

namespace CyberCity.Infrastructure
{
    public interface ILabRepository
    {
        Task<Lab> GetByIdAsync(string id);
        Task<List<Lab>> GetLabsByModuleIdAsync(string moduleId);
        Task<Lab> GetLabWithComponentsAsync(string labId);
        Task<List<LabComponent>> GetLabComponentsAsync(string labId);
    }

    public class LabRepo : GenericRepository<Lab>, ILabRepository
    {
        private readonly CyberCityLearningFlatFormDBContext _dbContext;

        public LabRepo(CyberCityLearningFlatFormDBContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<List<Lab>> GetLabsByModuleIdAsync(string moduleId)
        {
            return await _dbContext.Labs
                .Where(l => l.ModuleUid == moduleId)
                .OrderBy(l => l.OrderIndex)
                .ToListAsync();
        }

        public async Task<Lab> GetLabWithComponentsAsync(string labId)
        {
            return await _dbContext.Labs
                .Include(l => l.LabComponents)
                .FirstOrDefaultAsync(l => l.Uid == labId);
        }

        public async Task<List<LabComponent>> GetLabComponentsAsync(string labId)
        {
            return await _dbContext.LabComponents
                .Where(lc => lc.LabUid == labId)
                .OrderBy(lc => lc.OrderIndex)
                .ToListAsync();
        }
    }
}

