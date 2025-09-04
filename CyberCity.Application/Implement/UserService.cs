using CyberCity.Application.Interface;
using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Implement
{
    public class UserService : IUserService
    {
        private readonly UserRepo _userRepository;
        public UserService(UserRepo userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task<int> CreateAccount(User user)
        {
            return await _userRepository.CreateAsync(user);
        }

        public async Task<int> CreateAccountByAdmin(User user)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            return await _userRepository.CreateAsync(user);
        }

        public async Task<PagedResult<User>> GetAllAccounts(int pageNumber, int pageSize, bool descending = true)
        {
            var query = _userRepository.GetAllAsync(descending);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<User>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<User> GetUserAccountByNameOrEmailAsync(string EmailorName)
        {
            return await _userRepository.GetByUsernameOrEmailAsync(EmailorName);
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<int> UpdateAccount(User user)
        {
            return await _userRepository.UpdateAsync(user);
        }
        public async Task<bool> UpdatePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                var valid = BCrypt.Net.BCrypt.Verify(currentPassword ?? string.Empty, user.PasswordHash);
                if (!valid) return false;
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8) return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            var updated = await _userRepository.UpdateAsync(user);
            return updated > 0;
        }
    }
}
