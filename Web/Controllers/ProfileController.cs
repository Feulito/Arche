using Core.Models.ViewModels;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Helpers;
using Web.Models;

namespace Web.Controllers
{
    public class ProfileController : Controller
    {
        private IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile(string userId)
        {
            UserViewModel user = await ConnectionHelper.GetRights(User, _userService);
            if (user == null || user.Id != userId) return RedirectToAction("Index", "Index");

            return View(new ProfileViewModel()
            {
                User = user
            });
        }
    }
}
