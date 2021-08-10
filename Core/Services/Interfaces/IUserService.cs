using Core.Entities;
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
    public interface IUserService
    {
        Task<User> AddUser(User user);
        Task<User> GetUserById(string userId);
        Task<User> GetUserByNameAndMail(string userName, string Email);
        Task<User> GetUserByMail(string email);
        Task<User> SignIn(string email, string pass);
        Task DeleteById(string userId);
        Task Delete(User user);
        Task EditProfile(EditProfilFormData editProfileFormData);
    }
}
