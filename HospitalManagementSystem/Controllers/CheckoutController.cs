using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace HospitalManagementSystem.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IEmailService _emailService;

        public CheckoutController(IOrderRepository orderRepository, IProductRepository productRepository, IEmailService emailService)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            List<CartItem> cart = GetCart();

            if (cart.Count == 0)
            {
                TempData["Error"] = "Your cart is empty";
                return RedirectToAction("Index", "Shop");
            }

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string firstName, string lastName, string email, string phone, string address, string city, string zipCode, string notes, string paymentMethod)
        {
            List<CartItem> cart = GetCart();

            if (cart.Count == 0)
            {
                TempData["Error"] = "Your cart is empty";
                return RedirectToAction("Index", "Shop");
            }

            decimal totalAmount = 0;
            foreach (CartItem item in cart)
            {
                totalAmount = totalAmount + item.Total;
            }

            Order order = new Order {
              CustomerName = firstName + " " + lastName,
              CustomerEmail = email,
              CustomerPhone = phone,
              ShippingAddress = address + ", " + city + ", " + zipCode,
              TotalAmount = totalAmount,
              PaymentMethod = paymentMethod,
              Notes = notes,
              Status = "Pending",
              PaymentStatus = "Pending",
              OrderDate = DateTime.Now
            };
       

            if (User.Identity.IsAuthenticated)
            {
                order.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            order.Items = new List<OrderItem>();

            foreach (CartItem cartItem in cart)
            {
                OrderItem orderItem = new OrderItem
                {
                   ProductId = cartItem.ProductId,
                   ProductName = cartItem.ProductName,
                   Price = cartItem.Price,
                   Quantity = cartItem.Quantity,
                   Total = cartItem.Total
                };
              

                order.Items.Add(orderItem);
            }

            await _orderRepository.AddAsync(order);

            foreach (CartItem cartItem in cart)
            {
                Product product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                if (product != null)
                {
                    product.StockQuantity = product.StockQuantity - cartItem.Quantity;
                    await _productRepository.UpdateAsync(product);
                }
            }

            try
            {
                string subject = "Order Confirmation #" + order.Id;
                string body = "<h2>Thank you for your order!</h2>";
                body = body + "<p>Dear " + order.CustomerName + ",</p>";
                body = body + "<p>Your order has been received.</p>";
                body = body + "<p><strong>Order Number:</strong> #" + order.Id + "</p>";
                body = body + "<p><strong>Total:</strong> $" + order.TotalAmount + "</p>";
                body = body + "<p>Best regards,<br/>Hospital Pharmacy</p>";

                await _emailService.SendEmailAsync(email, subject, body);
            }
            catch
            {
            }

            HttpContext.Session.Remove("ShoppingCart");

            TempData["Success"] = "Your order has been placed successfully";
            return RedirectToAction("OrderSuccess", new { orderId = order.Id });
        }

        public async Task<IActionResult> OrderSuccess(int orderId)
        {
            Order order = await _orderRepository.GetByIdWithItemsAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IEnumerable<Order> orders = await _orderRepository.GetByUserIdAsync(userId);
            return View(orders);
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
    }
}
