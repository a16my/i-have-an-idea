using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using BirFikrimVar.Models;

namespace BirFikrimVar.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();
        public string ReturnUrl { get; set; } = string.Empty;

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                ModelState.AddModelError(string.Empty, ErrorMessage);

            returnUrl ??= Url.Content("~/");

            // clear any old external login cookies
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Look up user by email
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"User '{user.Email}' logged in.");
                        return LocalRedirect(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });

                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }

                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No account found with that email.");
                }
            }

            // redisplay form if failed
            return Page();
        }
    }
}