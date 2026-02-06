using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Repositories
{
    public class BlogCategoryRepository : IBlogCategoryRepository
    {
        private readonly AppDbContext _context;

        public BlogCategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BlogCategory>> GetAllAsync()
        {
            return await _context.BlogCategories
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogCategory>> GetAllWithBlogCountAsync()
        {
            return await _context.BlogCategories
                .Where(c => !c.IsDeleted)
                .Include(c => c.Blogs.Where(b => b.IsPublished && !b.IsDeleted))
                .ToListAsync();
        }

        public async Task<BlogCategory> GetByIdAsync(int id)
        {
            return await _context.BlogCategories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<BlogCategory> GetBySlugAsync(string slug)
        {
            return await _context.BlogCategories
                .FirstOrDefaultAsync(c => c.Slug == slug && !c.IsDeleted);
        }

        public async Task AddAsync(BlogCategory category)
        {
            await _context.BlogCategories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BlogCategory category)
        {
            _context.BlogCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            BlogCategory category = await _context.BlogCategories.FindAsync(id);
            if (category != null)
            {
                category.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
