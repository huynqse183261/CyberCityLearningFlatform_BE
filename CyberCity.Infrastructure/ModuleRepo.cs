using CyberCity.Doman.DBcontext;
using CyberCity.Doman.Models;
using CyberCity.Infrastructure.Basic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Infrastructure
{
    public class ModuleRepo: GenericRepository<Module>
    {
        public ModuleRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public ModuleRepo() { }
        public IQueryable<Module> GetAllAsync()
        {
            return _context.Modules
                .Include(m => m.CourseU)
                .OrderByDescending(m => m.OrderIndex)
                .AsQueryable();
        }
    }
}
