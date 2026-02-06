using HospitalManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces
{
    public interface IBlogCategoryRepository
    {
        Task<IEnumerable<BlogCategory>> GetAllAsync();
        Task<IEnumerable<BlogCategory>> GetAllWithBlogCountAsync();
        Task<BlogCategory> GetByIdAsync(int id);
        Task<BlogCategory> GetBySlugAsync(string slug);
        Task AddAsync(BlogCategory category);
        Task UpdateAsync(BlogCategory category);
        Task DeleteAsync(int id);
    }
}
