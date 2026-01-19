using HospitalManagement.Application.Dtos.Account;
using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IDepartmentRepository _departmentRepository;

        public AccountController(IAccountService accountService, IDepartmentRepository departmentRepository)
        {
            _accountService = accountService;
            _departmentRepository = departmentRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.LoginAsync(model);

            if (result.Success == false)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewBag.Departments = await _departmentRepository.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _departmentRepository.GetAllAsync();
                return View(model);
            }

            var result = await _accountService.RegisterAsync(model);

            if (result.Success == false)
            {
                ModelState.AddModelError("", result.Message);
                ViewBag.Departments = await _departmentRepository.GetAllAsync();
                return View(model);
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
