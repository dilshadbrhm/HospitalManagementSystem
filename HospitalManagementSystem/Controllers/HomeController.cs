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
        private readonly IEmailService _emailService;

        public HomeController(IHomeService homeService,IEmailService emailService)
        {
            _homeService = homeService;
            _emailService = emailService;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendContact(string name, string email, string phone, string subject, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
                {
                    TempData["Error"] = "Please fill all required fields";
                    return RedirectToAction("Contact");
                }

                string emailSubject = "Contact Form: " + (subject ?? "No Subject");
                string body = $@"
                    <h2>New Contact Message</h2>
                    <p><strong>Name:</strong> {name}</p>
                    <p><strong>Email:</strong> {email}</p>
                    <p><strong>Phone:</strong> {phone}</p>
                    <p><strong>Subject:</strong> {subject}</p>
                    <hr/>
                    <p><strong>Message:</strong></p>
                    <p>{message}</p>
                    <hr/>
                    <p><small>Sent at: {DateTime.Now:dd MMM yyyy HH:mm}</small></p>
                ";

                await _emailService.SendEmailAsync("dilsadib7@gmail.com", emailSubject, body);

                TempData["Success"] = "Your message has been sent successfully! We will contact you soon.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Something went wrong. Please try again later.";
            }

            return RedirectToAction("Contact");
        }
    }
}
