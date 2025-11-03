using CyberCity.Doman.Models;
using CyberCity.DTOs;
using CyberCity.DTOs.Lessons;
using CyberCity.DTOs.Topics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.Application.Interface
{
    public interface ITopicService
    {
        Task<PagedResult<TopicDetailDto>> GetTopicAsync(int pageNumber, int pageSize);
        Task<Topic> GetByIdAsync(string uid);
        Task<string> CreateAsync(Topic topic);
        Task<bool> UpdateAsync(Topic topic);
        Task<bool> DeleteAsync(string uid);
    }
}
