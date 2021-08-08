using Core.Entities;
using Core.Enums;
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
using System.Security.Authentication;
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

            HashObject hashObject = HasherUtility.Hash("test");
            User user = await _userService.AddUser(new User()
            {
                UserName = "Feulito",
                Email = "sebastienduterte@hotmail.fr",
                Password = hashObject.Hash,
                Salt = hashObject.Salt
            });

            User userDb = await _userService.GetUserById(user.Id);
            Assert.AreEqual(user.Id, userDb.Id);
            Assert.AreEqual(user.UserName, userDb.UserName);
            Assert.AreEqual(user.Password, userDb.Password);
            Assert.AreEqual(user.Salt, userDb.Salt);
        }

        [TestMethod]
        public async Task TestDeleteUserById()
        {
            HashObject hashObject = HasherUtility.Hash("test");
            User user = await _userService.AddUser(new User()
            {
                UserName = "user",
                Email = "user@arche-rp.fr",
                Password = hashObject.Hash,
                Salt = hashObject.Salt
            });
            Assert.IsNotNull(await _userService.GetUserById(user.Id));
            await _userService.DeleteById(user.Id);
            Assert.IsNull(await _userService.GetUserById(user.Id));

            user = await _userService.AddUser(new User()
            {
                UserName = "superAdmin",
                Email = "superadmin@arche-rp.fr",
                Password = hashObject.Hash,
                Salt = hashObject.Salt,
                ProfileType = EProfileType.SuperAdmin
            });
            Assert.IsNotNull(await _userService.GetUserById(user.Id));
            await Assert.ThrowsExceptionAsync<UserServiceException>(async () => await _userService.DeleteById(user.Id), "Impossible de supprimer un Super-Administrateur");
            Assert.IsNotNull(await _userService.GetUserById(user.Id));
        }

        [TestMethod]
        public async Task TestDeleteUser()
        {
            HashObject hashObject = HasherUtility.Hash("test");
            User user = await _userService.AddUser(new User()
            {
                UserName = "user",
                Email = "user@arche-rp.fr",
                Password = hashObject.Hash,
                Salt = hashObject.Salt
            });
            Assert.IsNotNull(await _userService.GetUserById(user.Id));
            await _userService.Delete(user);
            Assert.IsNull(await _userService.GetUserById(user.Id));

            user = await _userService.AddUser(new User()
            {
                UserName = "superAdmin",
                Email = "superadmin@arche-rp.fr",
                Password = hashObject.Hash,
                Salt = hashObject.Salt,
                ProfileType = EProfileType.SuperAdmin
            });
            Assert.IsNotNull(await _userService.GetUserById(user.Id));
            await Assert.ThrowsExceptionAsync<UserServiceException>(async () => await _userService.Delete(user), "Impossible de supprimer un Super-Administrateur");
            Assert.IsNotNull(await _userService.GetUserById(user.Id));
        }

        [TestMethod]
        public async Task TestGetUserByMail()
        {
            HashObject hashObject = HasherUtility.Hash("test");
            User user = await _userService.AddUser(new User()
            {
                UserName = "Feulito",
                Email = "sebastienduterte@hotmail.fr",
                Password = hashObject.Hash,
                Salt = hashObject.Salt
            });

            User userDb = await _userService.GetUserByMail(user.Email);
            Assert.AreEqual(user.Id, userDb.Id);
            Assert.AreEqual(user.UserName, userDb.UserName);
            Assert.AreEqual(user.Password, userDb.Password);
            Assert.AreEqual(user.Salt, userDb.Salt);
        }

        [TestMethod]
        public async Task TestGetUserByUserNameAndMail()
        {
            HashObject hashObject = HasherUtility.Hash("test");
            User user = await _userService.AddUser(new User()
            {
                UserName = "Feulito",
                Email = "sebastienduterte@hotmail.fr",
                Password = hashObject.Hash,
                Salt = hashObject.Salt
            });

            User userDb = await _userService.GetUserByNameAndMail(user.UserName, user.Email);
            Assert.AreEqual(user.Id, userDb.Id);
            Assert.AreEqual(user.UserName, userDb.UserName);
            Assert.AreEqual(user.Password, userDb.Password);
            Assert.AreEqual(user.Salt, userDb.Salt);
        }

        [TestMethod]
        public async Task TestAddUserError()
        {
            User user = null;
            await Assert.ThrowsExceptionAsync<UserServiceException>(async () => user = await _userService.AddUser(new User()
            {
                UserName = "Feulito",
            }), "Un utilisateur doit avoir un pseudo, un Email et un mot de passe.");
            Assert.IsNull(user);

            await Assert.ThrowsExceptionAsync<UserServiceException>(async () => user = await _userService.AddUser(new User()
            {
                Email = "sebastienduterte@hotmail.fr",
            }), "Un utilisateur doit avoir un pseudo, un Email et un mot de passe.");
            Assert.IsNull(user);

            await Assert.ThrowsExceptionAsync<UserServiceException>(async () => user = await _userService.AddUser(new User()
            {
                UserName = "Feulito",
                Email = "sebastienduterte@hotmail.fr",
            }), "Un utilisateur doit avoir un pseudo, un Email et un mot de passe.");
            Assert.IsNull(user);

            HashObject hashObject = HasherUtility.Hash("test");
            User feulito = await _userService.AddUser(new User()
            {
                UserName = "Feulito",
                Email = "sebastienduterte@hotmail.fr",
                Password = hashObject.Hash,
                Salt = hashObject.Salt
            });
            Assert.IsNotNull(feulito);

            await Assert.ThrowsExceptionAsync<UserServiceException>(async () => user = await _userService.AddUser(new User()
            {
                UserName = "Feulito",
                Email = "sebastienduterte@hotmail.fr"
            }), "Il existe déjà un utilisateur avec cet email");
        }

        [TestMethod]
        public async Task TestSignIn()
        {
            HashObject hashObject = HasherUtility.Hash("test");
            User feulito = await _userService.AddUser(new User()
            {
                UserName = "Feulito",
                Email = "sebastienduterte@hotmail.fr",
                Password = hashObject.Hash,
                Salt = hashObject.Salt
            });

            User user = await _userService.SignIn("sebastienduterte@hotmail.fr", "test");
            Assert.IsNotNull(user);

            await Assert.ThrowsExceptionAsync<AuthenticationException>(async () => await _userService.SignIn("truc", "test"), "Identifiants incorrects.");
            await Assert.ThrowsExceptionAsync<AuthenticationException>(async () => await _userService.SignIn("sebastienduterte@hotmail.fr", "machin"), "Identifiants incorrects.");
            await Assert.ThrowsExceptionAsync<AuthenticationException>(async () => await _userService.SignIn(null, "machin"), "Identifiants incorrects.");
            await Assert.ThrowsExceptionAsync<AuthenticationException>(async () => await _userService.SignIn("sebastienduterte@hotmail.fr", null), "Identifiants incorrects.");
            await Assert.ThrowsExceptionAsync<AuthenticationException>(async () => await _userService.SignIn(null, null), "Identifiants incorrects.");
        }
    }
}
