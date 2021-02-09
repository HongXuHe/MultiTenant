using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiTenant.Web
{
    public class HomeController:Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            var user = User;
            return View();
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }

}