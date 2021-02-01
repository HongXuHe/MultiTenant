﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenant.IdentityServerFour.Controllers
{
    public class HomeController:Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return Content("sec");
        }
    }
}