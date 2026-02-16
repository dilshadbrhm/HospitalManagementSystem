using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductCategoryController : Controller
    {
        private readonly IProductCategoryRepository _categoryRepository;

        public ProductCategoryController(IProductCategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<ProductCategory> categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategory category)
        {
            try
            {
                category.CreatedAt = DateTime.Now;
                await _categoryRepository.AddAsync(category);

                TempData["Success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }

            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ProductCategory category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductCategory category)
        {
            try
            {
                ProductCategory existingCategory = await _categoryRepository.GetByIdAsync(category.Id);

                if (existingCategory == null)
                {
                    return NotFound();
                }

                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
                existingCategory.ImageUrl = category.ImageUrl;
                existingCategory.UpdatedAt = DateTime.Now;

                await _categoryRepository.UpdateAsync(existingCategory);

                TempData["Success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            TempData["Success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
