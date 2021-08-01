using Core.Entities;
using Core.Services.Interfaces;
using Database;
using IOC;
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
        private IUserService _userService;

        [TestInitialize]
        public void Init()
        {
            Container.ServiceCollection.AddDbContext<ArcheDbContext>(o => o.UseInMemoryDatabase("ArcheTest"), ServiceLifetime.Transient);
            Container.RegisterAllTypes(ServiceLifetime.Transient);
            User user = new User(); // Nécéssite un premier appel pour que sont implémentation de la DAO soit disponible dans les services du container
            _userService = Container.Resolve<IUserService>();
        }

        [TestMethod]
        public void TestResolve()
        {
            Assert.IsNotNull(_userService);
        }

        [TestMethod]
        public async Task TestAddUser()
        {
            
            User user = await _userService.AddUser(new User()
            {
                UserName = "Feulito"
            });

            User userDb = await _userService.GetUserById(user.Id);
            Assert.AreEqual(user.Id, userDb.Id);
            Assert.AreEqual(user.UserName, userDb.UserName);
        }
    }
}
