using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenant.IdentityServerFour.Controllers
{
    public class ErrorController:Id4BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
