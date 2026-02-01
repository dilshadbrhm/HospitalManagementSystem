using HospitalManagement.Application.Dtos.Account;
using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace HospitalManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IEmailService _emailService;
        private readonly IDepartmentRepository _departmentRepository;

        public AccountController(
            IAccountService accountService,
            IEmailService emailService,
            IDepartmentRepository departmentRepository)
        {
            _accountService = accountService;
            _emailService = emailService;
            _departmentRepository = departmentRepository;
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

            if (result.Success)
            {
                var encodedToken = HttpUtility.UrlEncode(result.Token);
                var confirmationLink = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new { userId = result.UserId, token = encodedToken },
                    Request.Scheme);


                await _emailService.SendEmailAsync(
                    model.Email,
                    "Confirm Your Email",
                    $"<h1>Welcome!</h1><p>Please confirm your email by clicking <a href='{confirmationLink}'>here</a></p>");

                TempData["Success"] = "Registration successful! Please check your email to confirm your account.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", result.Message);
            ViewBag.Departments = await _departmentRepository.GetAllAsync();
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Invalid confirmation link";
                return RedirectToAction("Login");
            }

            var decodedToken = HttpUtility.UrlDecode(token);

            var result = await _accountService.ConfirmEmailAsync(userId, decodedToken);

            if (result.Success)
                TempData["Success"] = result.Message;
            else
                TempData["Error"] = result.Message;

            return RedirectToAction("Login");
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

            (bool Success, string Message, string Role) result = await _accountService.LoginAsync(model);

            if (result.Success)
            {
                if (result.Role == "Admin")
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else if (result.Role == "Doctor")
                {
                    return RedirectToAction("Cabinet", "Doctor");
                }
                else
                {
                    return RedirectToAction("Dashboard", "Patient");
                }
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.ForgotPasswordAsync(model.Email);

            if (result.Success && result.Token != null)
            {
                var resetLink = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { email = model.Email, token = result.Token },
                    Request.Scheme);

                await _emailService.SendEmailAsync(
                    model.Email,
                    "Reset Your Password",
                    $"<h1>Reset Password</h1><p>Click <a href='{resetLink}'>here</a> to reset your password.</p>");
            }

            TempData["Success"] = "If this email exists, a reset link will be sent.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            var model = new ResetPasswordDto
            {
                Email = email,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.ResetPasswordAsync(model);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
