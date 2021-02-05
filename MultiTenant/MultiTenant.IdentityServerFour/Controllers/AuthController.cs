using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MultiTenant.Entities;
using MultiTenant.IdentityServerFour.Models;
using System.Threading.Tasks;

namespace MultiTenant.IdentityServerFour.Controllers
{
    public class AuthController : Controller
    {
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly UserManager<Id4User> _userManager;
        private readonly SignInManager<Id4User> _signInManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IIdentityServerInteractionService interactionService,
                                UserManager<Id4User> userManager,
                                SignInManager<Id4User> signInManager,
                                ILogger<AuthController> logger)
        {

            _interactionService = interactionService;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult LogIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        /// <summary>
        /// login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.UserEmail);
                if (user != null &&user.SoftDelete==false)
                {
                    var signInRes = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
                    if (signInRes.Succeeded)
                    {
                        _logger.LogInformation("User {0} log in", user.Email);
                        if(model.ReturnUrl != null)
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        return Redirect("~/");
                    }
                    else
                    {
                        ModelState.AddModelError("UserName,Password Error", "Your UserName Or Password is Wrong");
                        ViewBag.ReturnUrl = model.ReturnUrl;
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("nonExistUser", "User is not exist");
                }
            }
            ViewBag.ReturnUrl = model.ReturnUrl;
            return View();
        }

        public async Task<IActionResult> Logout(string logoutId)
        {
            if (User?.Identity.IsAuthenticated != true)
            {
            }
            else
            {
                var context = await _interactionService.GetLogoutContextAsync(logoutId);
                _logger.LogInformation("User {0} log out", User.Identity.Name);
                await _signInManager.SignOutAsync();
               
                return Redirect(context.PostLogoutRedirectUri);
            }
            return View("Error");
        }

        public async Task<IActionResult> AccessDenied()
        {
            if (User?.Identity.IsAuthenticated == true)
            {
               await _signInManager.SignOutAsync();
            }
                return View();
        }
    }
}