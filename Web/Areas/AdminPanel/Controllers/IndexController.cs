using Core.Models.ViewModels;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Helpers;

namespace Web.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class IndexController : Controller
    {
        private readonly IUserService _userService;

        public IndexController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            UserViewModel user = await ConnectionHelper.GetRights(User, _userService);
            if (user == null || !user.IsAtLeastAdmin()) return Redirect("/Index");
            return View();
        }
    }
}
