using Core.Entities.Site;
using Core.Models;
using Core.Models.ViewModels;
using Core.Services.Interfaces;
using Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Helpers;
using Web.Models.Site;

namespace Web.Controllers
{
    public class IndexController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IUserService _userService;

        private static readonly int NbArticles = 3;

        public IndexController(IArticleService articleService, IUserService userService)
        {
            _articleService = articleService;
            _userService = userService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            await DataInitializer.Initialize(clear: false);
            UserViewModel user = User.Identity.IsAuthenticated ? await ConnectionHelper.GetRights(User, _userService) : null;

            List<ArticleViewModel> articles = MapperUtility.Map<Article, ArticleViewModel>((await _articleService.GetArticles(NbArticles)).OrderBy(a => a.Creation));
            IndexViewModel viewModel = new IndexViewModel() {
                Articles = articles,
                User = user
            };
            return View(viewModel);
        }
    }
}
