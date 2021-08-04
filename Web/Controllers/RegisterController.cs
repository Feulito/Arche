using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models.Site;

namespace Web.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            RegisterViewModel viewModel = new RegisterViewModel();
            return View(viewModel);
        }
    }
}
