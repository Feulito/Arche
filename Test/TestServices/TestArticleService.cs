using Core.Entities.Site;
using Core.Services.Interfaces;
using Database;
using IOC;
using IOC.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.TestServices
{
    [TestClass]
    public class TestArticleService
    {
        private static IArticleService _articleService;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Container.ServiceCollection.AddDbContext<ArcheDbContext>(o => o.UseInMemoryDatabase("ArcheTest"), ServiceLifetime.Transient);
            Container.RegisterAllTypes(ServiceLifetime.Transient);
            Article article = new Article(); // Nécéssite un premier appel pour que sont implémentation de la DAO soit disponible dans les services du container
            _articleService = Container.Resolve<IArticleService>();
        }

        [TestCleanup]
        public async Task CleanUpTests()
        {
            IAsyncDao<Article> articleDao = Container.Resolve<IAsyncDao<Article>>();
            await articleDao.DeleteAsync(await articleDao.ListAllAsync());
        }

        [TestMethod]
        public void TestResolveService()
        {
            Assert.IsNotNull(_articleService);
        }


        [TestMethod]
        public async Task TestAddArticle()
        {
            Article article = new Article()
            {
                Title = "Un titre",
                HeaderUrl = "https://www.publicdomainpictures.net/pictures/320000/velka/background-image.png",
                Content = "<p>Voici un paragraphe de mon article !</p>",
                AuteurId = "unId"
            };
            Article newArticle = await _articleService.AddArticle(article);
            Assert.IsNotNull(newArticle);
            Assert.AreEqual(article.Id, newArticle.Id);
            Assert.AreEqual(article.AuteurId, newArticle.AuteurId);
            Assert.AreEqual(article.Title, newArticle.Title);
            Assert.AreEqual(article.HeaderUrl, newArticle.HeaderUrl);
            Assert.AreEqual(article.Content, newArticle.Content);
            Assert.AreEqual(DateTime.Now.Date, newArticle.Creation.Date);
            Assert.IsFalse(newArticle.Deleted);

            Article dbArticle = await _articleService.GetArticleById(newArticle.Id);
            Assert.IsNotNull(dbArticle);
            Assert.AreEqual(newArticle.Id, dbArticle.Id);
            Assert.AreEqual(newArticle.AuteurId, dbArticle.AuteurId);
            Assert.AreEqual(newArticle.Title, dbArticle.Title);
            Assert.AreEqual(newArticle.HeaderUrl, dbArticle.HeaderUrl);
            Assert.AreEqual(newArticle.Content, dbArticle.Content);
            Assert.AreEqual(newArticle.Creation, dbArticle.Creation);
            Assert.IsFalse(dbArticle.Deleted);
        }

        [TestMethod]
        public async Task TestGetArticles()
        {
            for (int i = 0; i < 100; i++)
            {
                Article article = new Article()
                {
                    Title = $"Article {i}",
                    HeaderUrl = $"Header {i}",
                    Content = $"Content {i}",
                    AuteurId = $"Auteur {i}"
                };
                await _articleService.AddArticle(article);
            }

            List<Article> articles = await _articleService.GetArticles();
            articles = articles.OrderBy(a => a.Creation).ToList();
            Assert.IsTrue(articles.Count == 100);

            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual($"Article {i}", articles[i].Title);
                Assert.AreEqual($"Header {i}", articles[i].HeaderUrl);
                Assert.AreEqual($"Content {i}", articles[i].Content);
                Assert.AreEqual($"Auteur {i}", articles[i].AuteurId);
                Assert.IsFalse(articles[i].Deleted);
            }
        }

        [TestMethod]
        public async Task TestGetArticlesWithNbArticles()
        {
            for (int i = 0; i < 100; i++)
            {
                Article article = new Article()
                {
                    Title = $"Article {i}",
                    HeaderUrl = $"Header {i}",
                    Content = $"Content {i}",
                    AuteurId = $"Auteur {i}"
                };
                await _articleService.AddArticle(article);
            }

            List<Article> articles = await _articleService.GetArticles(3);
            Assert.IsTrue(articles.Count == 3);

            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual($"Article {i}", articles[i].Title);
                Assert.AreEqual($"Header {i}", articles[i].HeaderUrl);
                Assert.AreEqual($"Content {i}", articles[i].Content);
                Assert.AreEqual($"Auteur {i}", articles[i].AuteurId);
                Assert.IsFalse(articles[i].Deleted);
            }
        }

        [TestMethod]
        public async Task TestAddWithNullArticle()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await _articleService.AddArticle(null));
        }
    }
}
