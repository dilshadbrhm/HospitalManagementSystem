using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HospitalManagementSystem.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;

        public CartController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            List<CartItem> cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            if (quantity < 1)
            {
                quantity = 1;
            }

            Product product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            List<CartItem> cart = GetCart();

            CartItem existingItem = null;
            foreach (CartItem item in cart)
            {
                if (item.ProductId == productId)
                {
                    existingItem = item;
                    break;
                }
            }

            if (existingItem != null)
            {
                existingItem.Quantity = existingItem.Quantity + quantity;
            }
            else
            {
                CartItem newItem = new CartItem
                {
                   ProductId = product.Id,
                   ProductName = product.Name,
                   Image = product.ImageUrl,
                   Quantity = quantity
                };
               

                if (product.DiscountPrice.HasValue)
                {
                    newItem.Price = product.DiscountPrice.Value;
                }
                else
                {
                    newItem.Price = product.Price;
                }

                cart.Add(newItem);
            }

            SaveCart(cart);

            TempData["Success"] = "Product added to cart";
            return RedirectToAction("Index", "Shop");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            List<CartItem> cart = GetCart();

            foreach (CartItem item in cart)
            {
                if (item.ProductId == productId)
                {
                    if (quantity <= 0)
                    {
                        cart.Remove(item);
                    }
                    else
                    {
                        item.Quantity = quantity;
                    }
                    break;
                }
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            List<CartItem> cart = GetCart();

            CartItem itemToRemove = null;
            foreach (CartItem item in cart)
            {
                if (item.ProductId == productId)
                {
                    itemToRemove = item;
                    break;
                }
            }

            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                SaveCart(cart);
            }

            TempData["Success"] = "Product removed from cart";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("ShoppingCart");
            TempData["Success"] = "Cart cleared";
            return RedirectToAction("Index");
        }

        public IActionResult GetCartCount()
        {
            List<CartItem> cart = GetCart();
            int count = 0;

            foreach (CartItem item in cart)
            {
                count = count + item.Quantity;
            }

            return Json(new { count = count });
        }

        private List<CartItem> GetCart()
        {
            string cartJson = HttpContext.Session.GetString("ShoppingCart");

            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }

            List<CartItem> cart = JsonSerializer.Deserialize<List<CartItem>>(cartJson);
            return cart;
        }

        private void SaveCart(List<CartItem> cart)
        {
            string cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("ShoppingCart", cartJson);
        }
    }
}
