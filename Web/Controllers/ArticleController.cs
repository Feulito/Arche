using Core.Exceptions;
using Core.Models.ViewModels;
using Core.Services.Interfaces;
using Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            ArticleViewModel articleViewModel = MapperUtility.Map(await _articleService.GetArticleById(articleId), new ArticleViewModel());
            if (articleViewModel.Content.Contains("<script")) throw new ArticleServiceException("Pour des raisons de sécurité le code javascript n'est pas autorisé dans les articles !");
            ArticlePageViewModel viewModel = new ArticlePageViewModel()
            {
                Article = articleViewModel,
                Auteur = await _userService.GetUserById(articleViewModel.AuteurId)
            };
            return View(viewModel);
        }

        public async Task<IActionResult> AddArticle(string title, string content, string authorId)
        {

            return View();
        }

    }
}
