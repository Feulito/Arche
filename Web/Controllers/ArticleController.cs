using Core.Exceptions;
using Core.Models.ViewModels;
using Core.Services.Interfaces;
using Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Web.Helpers;
using Web.Models.Site;

namespace Web.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IUserService _userService;

        public ArticleController(IArticleService articleService, IUserService userService)
        {
            _articleService = articleService;
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index(string articleId)
        {
            UserViewModel user = await ConnectionHelper.GetRights(User, _userService);
            if (user == null) return RedirectToActionPermanent("Index", "Index");

            ArticleViewModel articleViewModel = MapperUtility.Map(await _articleService.GetArticleById(articleId), new ArticleViewModel());
            if (articleViewModel.Content.Contains("<script")) throw new ArticleServiceException("Pour des raisons de sécurité le code javascript n'est pas autorisé dans les articles !");
            ArticlePageViewModel viewModel = new ArticlePageViewModel()
            {
                Article = articleViewModel,
                Auteur = articleViewModel.Auteur,
                User = user
            };
            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> AddArticle(string title, string content, string authorId)
        {
            // test
            return View();
        }

    }
}
