using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace HospitalManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BlogCategoryController : Controller
    {
        private readonly IBlogCategoryRepository _categoryRepository;

        public BlogCategoryController(IBlogCategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<BlogCategory> categories = await _categoryRepository.GetAllWithBlogCountAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogCategory category)
        {
            if (string.IsNullOrEmpty(category.Name))
            {
                ModelState.AddModelError("Name", "Name is required");
                return View(category);
            }

            category.Slug = GenerateSlug(category.Name);

            await _categoryRepository.AddAsync(category);

            TempData["Success"] = "Category created successfully";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            BlogCategory category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BlogCategory category)
        {
            if (string.IsNullOrEmpty(category.Name))
            {
                ModelState.AddModelError("Name", "Name is required");
                return View(category);
            }

            BlogCategory existingCategory = await _categoryRepository.GetByIdAsync(category.Id);

            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.Name = category.Name;
            existingCategory.Slug = GenerateSlug(category.Name);
            existingCategory.Description = category.Description;

            await _categoryRepository.UpdateAsync(existingCategory);

            TempData["Success"] = "Category updated successfully";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            TempData["Success"] = "Category deleted successfully";
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
