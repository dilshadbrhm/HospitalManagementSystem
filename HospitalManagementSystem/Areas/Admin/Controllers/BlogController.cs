using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;

namespace HospitalManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BlogController : Controller
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IBlogCategoryRepository _categoryRepository;

        public BlogController(
            IBlogRepository blogRepository,
            IBlogCategoryRepository categoryRepository)
        {
            _blogRepository = blogRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Blog> blogs = await _blogRepository.GetAllAsync();
            return View(blogs);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            IEnumerable<BlogCategory> categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog, IFormFile image)
        {
            if (string.IsNullOrEmpty(blog.Title))
            {
                ModelState.AddModelError("Title", "Title is required");
            }

            if (blog.CategoryId == 0)
            {
                ModelState.AddModelError("CategoryId", "Category is required");
            }

            if (!ModelState.IsValid)
            {
                IEnumerable<BlogCategory> categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                return View(blog);
            }

            if (image != null && image.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "image", "blog");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                blog.ImageUrl = "/assets/image/blog/" + uniqueFileName;
            }

            blog.Slug = GenerateSlug(blog.Title);
            blog.PublishedDate = DateTime.Now;
            blog.ViewCount = 0;

            await _blogRepository.AddAsync(blog);

            TempData["Success"] = "Blog created successfully";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Blog blog = await _blogRepository.GetByIdAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            IEnumerable<BlogCategory> categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", blog.CategoryId);

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Blog blog, IFormFile image)
        {
            if (string.IsNullOrEmpty(blog.Title))
            {
                ModelState.AddModelError("Title", "Title is required");
            }

            if (!ModelState.IsValid)
            {
                IEnumerable<BlogCategory> categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", blog.CategoryId);
                return View(blog);
            }

            Blog existingBlog = await _blogRepository.GetByIdAsync(blog.Id);

            if (existingBlog == null)
            {
                return NotFound();
            }

            if (image != null && image.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "image", "blog");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                existingBlog.ImageUrl = "/assets/image/blog/" + uniqueFileName;
            }

            existingBlog.Title = blog.Title;
            existingBlog.Slug = GenerateSlug(blog.Title);
            existingBlog.ShortDescription = blog.ShortDescription;
            existingBlog.Content = blog.Content;
            existingBlog.CategoryId = blog.CategoryId;
            existingBlog.AuthorName = blog.AuthorName;
            existingBlog.IsPublished = blog.IsPublished;

            await _blogRepository.UpdateAsync(existingBlog);

            TempData["Success"] = "Blog updated successfully";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _blogRepository.DeleteAsync(id);
            TempData["Success"] = "Blog deleted successfully";
            return RedirectToAction("Index");
        }

        private string GenerateSlug(string title)
        {
            string slug = title.ToLower();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"-+", "-");
            slug = slug.Trim('-');
            return slug;
        }
    }
}
