using Core.Entities.Site;
using Core.Exceptions;
using Core.Models.FormData;
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
        private readonly IArticleService _articleService;

        public ArticleController(IUserService userService, IArticleService articleService)
        {
            _userService = userService;
            _articleService = articleService;
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> NewArticle(AddArticleFormData addNewArticleFormData)
        {
            UserViewModel user = await ConnectionHelper.GetRights(User, _userService);
            if (user == null || !user.IsAtLeastAdmin()) return Redirect("/Index");

            Article article;
            try
            {
                article = await _articleService.AddArticle(addNewArticleFormData);
            } catch (ArticleServiceException e)
            {
                return View(new ArticlePageViewModel() { User = user, ErrorMessage = e.Message });
            }

            return Redirect($"/Article?articleId={article.Id}");
        }

    }
}
