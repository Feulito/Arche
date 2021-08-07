using Core.Models.ViewModels;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Areas.AdminPanel.Models;
using Web.Helpers;

namespace Web.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class ArticleController : Controller
    {
        private readonly IUserService _userService;

        public ArticleController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        public IActionResult Index()
        {
            return RedirectToAction("NewArticle");
        }

        [Authorize]
        public async Task<IActionResult> NewArticle()
        {
            UserViewModel user = await ConnectionHelper.GetRights(User, _userService);
            if (user == null || !user.IsAtLeastAdmin()) return Redirect("/Index");
            return View(new ArticlePageViewModel() { User = user });
        }

    }
}
