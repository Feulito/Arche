using Core.Entities.Site;
using Core.Exceptions;
using Core.Models.FormData;
using Core.Services.Interfaces;
using Core.Utils;
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

        public async Task<Article> AddArticle(AddArticleFormData articleFormData)
        {
            CheckArticleFormDataInfos(articleFormData);
            Article article = new Article()
            {
                Title = articleFormData.Title,
                HeaderUrl = articleFormData.HeaderUrl,
                Content = articleFormData.Content,
                AuteurId = articleFormData.AuteurId
            };

            return await _articleDao.AddAsync(article);
        }

        private void CheckArticleFormDataInfos(AddArticleFormData articleFormData)
        {
            if (articleFormData == null) throw new ArgumentNullException("L'ajout d'un article null n'est pas autorisée.");
            if (string.IsNullOrWhiteSpace(articleFormData.AuteurId)) throw new ArticleServiceException("Un article doit avoir un auteur.");
            if (string.IsNullOrWhiteSpace(articleFormData.Content)) throw new ArticleServiceException("Un article doit avoir un contenu.");
            if (string.IsNullOrWhiteSpace(articleFormData.HeaderUrl)) throw new ArticleServiceException("Un article doit avoir une image d'entête.");
            if (string.IsNullOrWhiteSpace(articleFormData.Title)) throw new ArticleServiceException("Un article doit avoir un Titre.");
        }

        public async Task<Article> GetArticleById(string articleId)
        {
            ISpecification<Article> spec = new Specification<Article>()
            {
                Criteria = a => !a.Deleted && a.Id == articleId,
                IncludeExpressions = new List<System.Linq.Expressions.Expression<Func<Article, object>>>()
                {
                    a => a.Auteur
                }
            };

            return await _articleDao.FirstOrDefaultAsync(spec);
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
