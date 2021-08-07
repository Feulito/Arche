using Core.Entities.Site;
using Core.Models;
using Core.Models.ViewModels;
using Core.Services.Interfaces;
using Core.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models.Site;

namespace Web.Controllers
{
    public class IndexController : Controller
    {
        private readonly IArticleService _articleService;

        private static readonly int NbArticles = 3;

        public IndexController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        public async Task<IActionResult> Index()
        {
            await DataInitializer.Initialize(true);

            List<ArticleViewModel> articles = MapperUtility.Map<Article, ArticleViewModel>((await _articleService.GetArticles(NbArticles)).OrderBy(a => a.Creation));
            IndexViewModel viewModel = new IndexViewModel() {
                Articles = articles
            };
            return View(viewModel);
        }
    }
}
