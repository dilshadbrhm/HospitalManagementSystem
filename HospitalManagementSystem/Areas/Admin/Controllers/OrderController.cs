using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Order> orders = await _orderRepository.GetAllAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            Order order = await _orderRepository.GetByIdWithItemsAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await _orderRepository.UpdateStatusAsync(id, status);
            TempData["Success"] = "Order status updated";
            return RedirectToAction("Details", new { id = id });
        }
    }
}
