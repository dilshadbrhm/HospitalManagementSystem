using HospitalManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces
{
    public interface IBlogRepository
    {
        Task<IEnumerable<Blog>> GetAllAsync();
        Task<IEnumerable<Blog>> GetPublishedAsync();
        Task<IEnumerable<Blog>> GetByCategoryAsync(int categoryId);
        Task<Blog> GetByIdAsync(int id);
        Task<Blog> GetBySlugAsync(string slug);
        Task<IEnumerable<Blog>> SearchAsync(string searchTerm);
        Task<IEnumerable<Blog>> GetRecentAsync(int count);
        Task AddAsync(Blog blog);
        Task UpdateAsync(Blog blog);
        Task DeleteAsync(int id);
        Task IncrementViewCountAsync(int id);
    }
}
