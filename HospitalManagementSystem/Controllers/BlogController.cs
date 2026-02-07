using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
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
        public async Task<IActionResult> Index(int? categoryId, string search)
        {
            IEnumerable<Blog> blogs;

            if (!string.IsNullOrEmpty(search))
            {
                blogs = await _blogRepository.SearchAsync(search);
                ViewBag.SearchTerm = search;
            }
            else if (categoryId.HasValue)
            {
                blogs = await _blogRepository.GetByCategoryAsync(categoryId.Value);
                ViewBag.SelectedCategoryId = categoryId.Value;
            }
            else
            {
                blogs = await _blogRepository.GetPublishedAsync();
            }

            IEnumerable<BlogCategory> categories = await _categoryRepository.GetAllWithBlogCountAsync();
            IEnumerable<Blog> recentPosts = await _blogRepository.GetRecentAsync(5);

            ViewBag.Categories = categories;
            ViewBag.RecentPosts = recentPosts;

            return View(blogs);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return RedirectToAction("Index");
            }

            Blog blog = await _blogRepository.GetBySlugAsync(slug);

            if (blog == null)
            {
                return NotFound();
            }

            await _blogRepository.IncrementViewCountAsync(blog.Id);

            IEnumerable<BlogCategory> categories = await _categoryRepository.GetAllWithBlogCountAsync();
            IEnumerable<Blog> recentPosts = await _blogRepository.GetRecentAsync(5);

            ViewBag.Categories = categories;
            ViewBag.RecentPosts = recentPosts;

            return View(blog);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(BlogCategory category)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(category.Name))
        //        {
        //            TempData["Error"] = "Name is required";
        //            return View(category);
        //        }

        //        category.Slug = GenerateSlug(category.Name);
        //        category.CreatedDate = DateTime.Now;
        //        category.IsDeleted = false;

        //        await _categoryRepository.AddAsync(category);

        //        TempData["Success"] = "Category created successfully";
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        string error = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        TempData["Error"] = "Error: " + error;
        //        return View(category);
        //    }
        //}
    }
}
