using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiTenant.IdentityServerFour.Models;
using System;
using System.Threading.Tasks;

namespace MultiTenant.IdentityServerFour.Controllers
{
    public class AuthController : Controller
    {
        private readonly TestUserStore _testUserStore;
        private readonly IIdentityServerInteractionService _interactionService;
        public AuthController(TestUserStore testUserStore, IIdentityServerInteractionService interactionService)
        {
            _interactionService = interactionService;
            _testUserStore = testUserStore;

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
                var user = _testUserStore.FindByUsername(model.UserName);
                if (user != null)
                {
                    var IsVaildUser = _testUserStore.ValidateCredentials(model.UserName, model.Password);
                    if (IsVaildUser)
                    {
                        var identityUser = new IdentityServerUser(user.SubjectId)
                        {
                            DisplayName = user.Username
                        };
                        await HttpContext.SignInAsync(identityUser, new AuthenticationProperties() { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) });
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("user or password not correct", "user or password not correct");
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
                
                await HttpContext.SignOutAsync();
                return Redirect(context.PostLogoutRedirectUri);
            }
            return View("Error");
        }
    }
}