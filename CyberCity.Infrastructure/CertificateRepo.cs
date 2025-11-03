using CyberCity.Doman.DBContext;
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
    public class CertificateRepo : GenericRepository<Certificate>
    {
        public CertificateRepo() { }
        public CertificateRepo(CyberCityLearningFlatFormDBContext context) => _context = context;
        public IQueryable<Certificate> GetAllAsync()
        {
            var query = _context.Certificates.AsQueryable();
            return query.OrderByDescending(c => c.IssuedAt);
        }
        public async Task<List<Certificate>> GetByUserUidAsync(string userUid)
        {
            return await _context.Certificates.Where(c => c.UserUid == userUid).ToListAsync();
        }
    }
}
