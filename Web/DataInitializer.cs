using Core.Entities;
using Core.Entities.Site;
using Core.Enums;
using Core.Utils;
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
                await articleDao.DeleteAsync(await articleDao.ListAllAsync());
                await userDao.DeleteAsync(await userDao.ListAllAsync());
            }

            if ((await userDao.ListAllAsync()).Any()) return;

            HashObject hashObject = HasherUtility.Hash("test");
            User superAdmin = new User()
            {
                UserName = "SuperAdmin",
                Email = "superadmin@arche-rp.fr",
                Password = hashObject.Hash,
                Salt = hashObject.Salt,
                ProfileType = EProfileType.SuperAdmin
            };
            await userDao.AddAsync(superAdmin);

        }

    }
}
