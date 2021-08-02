using Core.Entities.Site;
using Core.Services.Interfaces;
using IOC.Data.Implementations;
using IOC.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Implementations
{
    public class ArticleService : IArticleService
    {
        private readonly IAsyncDao<Article> _articleDao;

        public ArticleService(IAsyncDao<Article> articleDao)
        {
            _articleDao = articleDao;
        }

        public async Task<Article> AddArticle(Article article)
        {
            if (article == null) throw new ArgumentNullException("article", "L'article à ajouter ne doit pas être null");
            await _articleDao.AddAsync(article);
            return article;
        }

        public async Task<Article> GetArticleById(string articleId)
        {
            return await _articleDao.GetByIdAsync(articleId);
        }

        public async Task<List<Article>> GetArticles()
        {
            ISpecification<Article> spec = new Specification<Article>()
            {
                Criteria = a => !a.Deleted
            };
            return (await _articleDao.ListAsync(spec)).ToList();
        }

        public async Task<List<Article>> GetArticles(int nbArticles)
        {
            ISpecification<Article> spec = new Specification<Article>()
            {
                Criteria = a => !a.Deleted,
                Take = nbArticles,
                OrderBy = a => a.Creation,
                IsPagingEnabled = true
            };
            return (await _articleDao.ListAsync(spec)).ToList();
        }
    }
}
