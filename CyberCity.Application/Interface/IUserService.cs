using CyberCity.Doman.Models;
using CyberCity.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface IUserService
    {
        Task<int> CreateAccount(User user);
        Task<int> CreateAccountByAdmin(User user);
        Task<int> UpdateAccount(User user);
        Task<PagedResult<User>> GetAllAccounts(int pageNumber, int pageSize, bool descending = true);
        Task<User> GetUserAccountByNameOrEmailAsync(string EmailorName);
        Task<User> GetByIdAsync(string id);
        Task<bool> UpdatePasswordAsync(string userId, string currentPassword, string newPassword);
    }
}
