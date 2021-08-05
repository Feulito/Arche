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
        Task<User> AddUser(User user);
        Task<User> GetUserById(string userId);
        Task<User> GetUserByNameAndMail(string userName, string Email);
        Task<User> SignIn(string email, string pass);
    }
}
