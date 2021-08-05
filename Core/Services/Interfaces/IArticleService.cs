using Core.Entities.Site;
using Core.Models.FormData;
using IOC.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Interfaces
{
    [Resolvable]
    public interface IArticleService
    {
        /// <summary>
        /// Ajoute un article dans la base de donnée
        /// </summary>
        /// <param name="article">L'article à ajouter</param>
        /// <returns>L'article ajouté</returns>
        Task<Article> AddArticle(AddArticleFormData article);

        /// <summary>
        /// Récupère tous les articles non supprimés
        /// </summary>
        /// <returns></returns>
        Task<List<Article>> GetArticles();

        /// <summary>
        /// Récupère un les derniers nbArticles
        /// </summary>
        /// <param name="nbArticles"></param>
        /// <returns></returns>
        Task<List<Article>> GetArticles(int nbArticles);

        /// <summary>
        /// Retourne l'article identifié par articleId
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        Task<Article> GetArticleById(string articleId);
    }
}
