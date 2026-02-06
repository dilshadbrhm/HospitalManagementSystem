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
    public class BlogRepository : IBlogRepository
    {
        private readonly AppDbContext _context;

        public BlogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Blog>> GetAllAsync()
        {
            return await _context.Blogs
                .Include(b => b.Category)
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.PublishedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Blog>> GetPublishedAsync()
        {
            return await _context.Blogs
                .Where(b => b.IsPublished && !b.IsDeleted)
                .Include(b => b.Category)
                .OrderByDescending(b => b.PublishedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Blog>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Blogs
                .Where(b => b.CategoryId == categoryId && b.IsPublished && !b.IsDeleted)
                .Include(b => b.Category)
                .OrderByDescending(b => b.PublishedDate)
                .ToListAsync();
        }

        public async Task<Blog> GetByIdAsync(int id)
        {
            return await _context.Blogs
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        }

        public async Task<Blog> GetBySlugAsync(string slug)
        {
            return await _context.Blogs
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Slug == slug && b.IsPublished && !b.IsDeleted);
        }

        public async Task<IEnumerable<Blog>> SearchAsync(string searchTerm)
        {
            return await _context.Blogs
                .Where(b => b.IsPublished && !b.IsDeleted &&
                    (b.Title.Contains(searchTerm) || b.Content.Contains(searchTerm)))
                .Include(b => b.Category)
                .OrderByDescending(b => b.PublishedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Blog>> GetRecentAsync(int count)
        {
            return await _context.Blogs
                .Where(b => b.IsPublished && !b.IsDeleted)
                .Include(b => b.Category)
                .OrderByDescending(b => b.PublishedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task AddAsync(Blog blog)
        {
            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Blog blog)
        {
            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Blog blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                blog.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task IncrementViewCountAsync(int id)
        {
            Blog blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                blog.ViewCount = blog.ViewCount + 1;
                await _context.SaveChangesAsync();
            }
        }
    }
}
