using HospitalManagement.Domain.Entities;
using HospitalManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IWebHostEnvironment _env;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly AppDbContext _context;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IWebHostEnvironment env,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
            _roleManager = roleManager;
            _context = context;
        }


        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

    }
}
