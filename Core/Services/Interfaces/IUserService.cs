using Core.Entities;
using IOC.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Interfaces
{
    [Resolvable]
    public interface IUserService
    {
        public Task<User> AddUser(User user);
        public Task<User> GetUserById(string userId);
        Task<User> GetUserByNameAndMail(string userName, string Email);
    }
}
