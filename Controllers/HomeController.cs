using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BirFikrimVar.Models;

namespace BirFikrimVar.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Root page "/"
        public IActionResult Index()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                // Not logged in → go to Login
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Logged in → show feed
            return RedirectToAction("Index", "Posts");
        }

        public IActionResult Page()
        {
            return View();
        }
    }
}