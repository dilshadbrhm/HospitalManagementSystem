using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    public class ShopController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductCategoryRepository _categoryRepository;

        public ShopController(IProductRepository productRepository, IProductCategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(int? categoryId, string search)
        {
            IEnumerable<Product> products;

            if (!string.IsNullOrEmpty(search))
            {
                products = await _productRepository.SearchAsync(search);
                ViewBag.SearchTerm = search;
            }
            else if (categoryId.HasValue)
            {
                products = await _productRepository.GetByCategoryAsync(categoryId.Value);
                ViewBag.SelectedCategoryId = categoryId.Value;
            }
            else
            {
                products = await _productRepository.GetActiveAsync();
            }

            IEnumerable<ProductCategory> categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = categories;

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            Product product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            IEnumerable<Product> relatedProducts = await _productRepository.GetByCategoryAsync(product.CategoryId);
            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }
    }
}
