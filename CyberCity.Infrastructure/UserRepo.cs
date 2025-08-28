using BloodDonation.Repositories.NhanNB.Basic;
using CyberCity.Doman.DBcontext;
using CyberCity.Doman.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Infrastructure
{
    public class UserRepo : GenericRepository<User>
    {
        public UserRepo() { }
        public UserRepo(CyberCityLearningFlatFormDBContext context) => _context = context;

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email.Trim());
        }
        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username.Trim());
        }
        public async Task<User> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            var key = usernameOrEmail.Trim();
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == key || u.Email == key);
        }
        public IQueryable<User> GetAllAsync()
        {
            return _context.Users.Include(u => u.Uid)
                .OrderByDescending(u => u.CreatedAt);
        }
    }
}
