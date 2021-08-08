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
using Web.Models;

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
        [AllowAnonymous]
        public async Task<IActionResult> Index(string articleId)
        {
            UserViewModel user = User.Identity.IsAuthenticated ? await ConnectionHelper.GetRights(User, _userService) : null;

            ArticleViewModel articleViewModel = MapperUtility.Map(await _articleService.GetArticleById(articleId), new ArticleViewModel());
            if (articleViewModel.Content.Contains("<script")) return ValidationProblem("Pour des raisons de sécurité le code javascript n'est pas autorisé dans les articles !");

            ArticlePageViewModel viewModel = new ArticlePageViewModel()
            {
                Article = articleViewModel,
                Auteur = articleViewModel.Auteur,
                User = user
            };
            return View(viewModel);
        }

    }
}
