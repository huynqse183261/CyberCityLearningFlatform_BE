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
        public IQueryable<User> GetAllAsync(bool descending = true)
        {
            var query = _context.Users.AsQueryable();

            return descending
                ? query.OrderByDescending(u => u.CreatedAt)
                : query.OrderBy(u => u.CreatedAt);
        }
        public async Task<User> GetUserAsyncbyId(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Uid == id);
        }
    }
}
