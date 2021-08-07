using Core.Entities;
using Core.Entities.Site;
using Core.Exceptions;
using Core.Models.FormData;
using Core.Services.Interfaces;
using Core.Utils;
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
        private static IUserService _userService;

        private static User Auteur { get; set; }

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Container.ServiceCollection.AddDbContext<ArcheDbContext>(o => o.UseInMemoryDatabase("ArcheTest"), ServiceLifetime.Transient);
            Container.RegisterAllTypes(ServiceLifetime.Transient);
            Article article = new Article(); // Nécéssite un premier appel pour que sont implémentation de la DAO soit disponible dans les services du container
            _articleService = Container.Resolve<IArticleService>();
            _userService = Container.Resolve<IUserService>();
            HashObject hashObject = HasherUtility.Hash("test");
            Auteur = new User()
            {
                UserName = "Feulito",
                Email = "sebastienduterte@hotmail.fr",
                Password = hashObject.Hash,
                Salt = hashObject.Salt
            };
            _userService.AddUser(Auteur);
        }

        [TestCleanup]
        public async Task CleanUpTests()
        {
            IAsyncDao<Article> articleDao = Container.Resolve<IAsyncDao<Article>>();
            await articleDao.DeleteAsync(await articleDao.ListAllAsync());
            IAsyncDao<User> userDao = Container.Resolve<IAsyncDao<User>>();
            await userDao.DeleteAsync(await userDao.ListAllAsync());
        }

        [ClassCleanup]
        public static async Task CleanUp()
        {
            IAsyncDao<Article> articleDao = Container.Resolve<IAsyncDao<Article>>();
            await articleDao.DeleteAsync(await articleDao.ListAllAsync());
            IAsyncDao<User> userDao = Container.Resolve<IAsyncDao<User>>();
            await userDao.DeleteAsync(await userDao.ListAllAsync());
        }

        [TestMethod]
        public void TestResolveService()
        {
            Assert.IsNotNull(_articleService);
        }


        [TestMethod]
        public async Task TestAddArticle()
        {
            AddArticleFormData article = new AddArticleFormData()
            {
                Title = "Un titre",
                HeaderUrl = "https://www.publicdomainpictures.net/pictures/320000/velka/background-image.png",
                Content = "<p>Voici un paragraphe de mon article !</p>",
                AuteurId = Auteur.Id
            };
            Article newArticle = await _articleService.AddArticle(article);
            Assert.IsNotNull(newArticle);
            Assert.AreEqual(Auteur.Id, article.AuteurId);
            Assert.AreEqual("Un titre", newArticle.Title);
            Assert.AreEqual("https://www.publicdomainpictures.net/pictures/320000/velka/background-image.png", newArticle.HeaderUrl);
            Assert.AreEqual("<p>Voici un paragraphe de mon article !</p>", newArticle.Content);

            Article dbArticle = await _articleService.GetArticleById(newArticle.Id);
            Assert.IsNotNull(dbArticle);
            Assert.AreEqual(newArticle.Id, dbArticle.Id);
            Assert.AreEqual(newArticle.AuteurId, Auteur.Id);
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
                AddArticleFormData article = new AddArticleFormData()
                {
                    Title = $"Article {i}",
                    HeaderUrl = $"Header {i}",
                    Content = $"Content {i}",
                    AuteurId = Auteur.Id
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
                Assert.AreEqual(Auteur.Id, articles[i].AuteurId);
                Assert.IsFalse(articles[i].Deleted);
            }
        }

        [TestMethod]
        public async Task TestGetArticlesWithNbArticles()
        {
            for (int i = 0; i < 100; i++)
            {
                AddArticleFormData article = new AddArticleFormData()
                {
                    Title = $"Article {i}",
                    HeaderUrl = $"Header {i}",
                    Content = $"Content {i}",
                    AuteurId = Auteur.Id
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
                Assert.AreEqual(Auteur.Id, articles[i].AuteurId);
                Assert.IsFalse(articles[i].Deleted);
            }
        }

        [TestMethod]
        public async Task TestAddArticleErrors()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await _articleService.AddArticle(null), "L'ajout d'un article null n'est pas autorisée.");
            AddArticleFormData datas = new AddArticleFormData();
            await Assert.ThrowsExceptionAsync<ArticleServiceException>(async () => await _articleService.AddArticle(datas), "Un article doit avoir un auteur.");
            datas.AuteurId = "Un auteur";
            await Assert.ThrowsExceptionAsync<ArticleServiceException>(async () => await _articleService.AddArticle(datas), "Un article doit avoir un contenu.");
            datas.Content = "<p>Truc</p>";
            await Assert.ThrowsExceptionAsync<ArticleServiceException>(async () => await _articleService.AddArticle(datas), "Un article doit avoir une image d'entête.");
            datas.HeaderUrl = "une image entete";
            await Assert.ThrowsExceptionAsync<ArticleServiceException>(async () => await _articleService.AddArticle(datas), "Un article doit avoir un titre.");
            datas.Title = "Un titre";
            Article article = await _articleService.AddArticle(datas);
            Assert.IsNotNull(article);
        }
    }
}
