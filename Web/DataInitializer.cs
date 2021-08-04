using Core.Entities;
using Core.Entities.Site;
using IOC;
using IOC.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
    public static class DataInitializer
    {

        public static async Task Initialize(bool clear)
        {
            IAsyncDao<User> userDao = Container.Resolve<IAsyncDao<User>>();
            IAsyncDao<Article> articleDao = Container.Resolve<IAsyncDao<Article>>();

            if (clear)
            {
                await userDao.DeleteAsync(await userDao.ListAllAsync());
                await articleDao.DeleteAsync(await articleDao.ListAllAsync());
            }

            if ((await userDao.ListAllAsync()).Any()) return;

            User feulito = new User()
            {
                UserName = "Feulito",
                Email = "sebastienduterte@hotmail.fr"
            };
            await userDao.AddAsync(feulito);

            Article article = new Article()
            {
                AuteurId = feulito.Id,
                Title = "Premier article !",
                HeaderUrl = "https://www.publicdomainpictures.net/pictures/320000/velka/background-image.png",
                Content = "<p>Un premier article !</p>"
            };
            await articleDao.AddAsync(article);

            article = new Article()
            {
                AuteurId = feulito.Id,
                Title = "Deuxième article !",
                HeaderUrl = "https://static.remove.bg/remove-bg-web/3661dd45c31a4ff23941855a7e4cedbbf6973643/assets/start-0e837dcc57769db2306d8d659f53555feb500b3c5d456879b9c843d1872e7baa.jpg",
                Content = "<p>Un deuxième article !</p>"
            };
            await articleDao.AddAsync(article);

            article = new Article()
            {
                AuteurId = feulito.Id,
                Title = "Troisième article !",
                HeaderUrl = "https://media.sproutsocial.com/uploads/2017/02/10x-featured-social-media-image-size.png",
                Content = "<p>Un troisième article !</p>"
            };
            await articleDao.AddAsync(article);

            article = new Article()
            {
                AuteurId = feulito.Id,
                Title = "Quatrième article !",
                HeaderUrl = "https://interactive-examples.mdn.mozilla.net/media/cc0-images/grapefruit-slice-332-332.jpg",
                Content = "<p>Un quatrième article !</p>"
            };
            await articleDao.AddAsync(article);
        }

    }
}
