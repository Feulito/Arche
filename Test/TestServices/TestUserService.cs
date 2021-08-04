using Core.Entities;
using Core.Exceptions;
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
    public class TestUserService
    {
        private static IUserService _userService;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Container.ServiceCollection.AddDbContext<ArcheDbContext>(o => o.UseInMemoryDatabase("ArcheTest"), ServiceLifetime.Transient);
            Container.RegisterAllTypes(ServiceLifetime.Transient);
            User user = new User(); // Nécéssite un premier appel pour que sont implémentation de la DAO soit disponible dans les services du container
            _userService = Container.Resolve<IUserService>();
        }

        [TestCleanup]
        public async Task CleanTest()
        {
            IAsyncDao<User> userDao = Container.Resolve<IAsyncDao<User>>();
            await userDao.DeleteAsync(await userDao.ListAllAsync());
        }

        [TestMethod]
        public void TestResolveService()
        {
            Assert.IsNotNull(_userService);
        }

        [TestMethod]
        public async Task TestAddUser()
        {
            
            User user = await _userService.AddUser(new User()
            {
                UserName = "Feulito",
                Email = "sebastienduterte@hotmail.fr"
            });

            User userDb = await _userService.GetUserById(user.Id);
            Assert.AreEqual(user.Id, userDb.Id);
            Assert.AreEqual(user.UserName, userDb.UserName);
        }

        [TestMethod]
        public async Task TestAddUserError()
        {
            User user = null;
            await Assert.ThrowsExceptionAsync<UserServiceException>(async () => user = await _userService.AddUser(new User()
            {
                UserName = "Feulito",
            }), "Un utilisateur doit avoir un pseudo et un email");
            Assert.IsNull(user);

            await Assert.ThrowsExceptionAsync<UserServiceException>(async () => user = await _userService.AddUser(new User()
            {
                Email = "sebastienduterte@hotmail.fr",
            }), "Un utilisateur doit avoir un pseudo et un email");
            Assert.IsNull(user);

            User feulito = await _userService.AddUser(new User()
            {
                UserName = "Feulito",
                Email = "sebastienduterte@hotmail.fr"
            });
            Assert.IsNotNull(feulito);

            await Assert.ThrowsExceptionAsync<UserServiceException>(async () => user = await _userService.AddUser(new User()
            {
                UserName = "Feulito",
                Email = "sebastienduterte@hotmail.fr"
            }), "Il existe déjà un utilisateur avec cet email");
        }
    }
}
