using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain;
using HospitalManagement.Infrastructure.Persistence;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HospitalManagementSystem.Controllers
{
    public class HomeController : Controller
    {

        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _homeService.GetHomeDataAsync();
            return View(viewModel);
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }


    }
}
