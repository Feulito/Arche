using Core.Entities;
using Core.Services.Interfaces;
using IOC.Data.Implementations;
using IOC.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IAsyncDao<User> _userDao;

        public UserService(IAsyncDao<User> userDao)
        {
            _userDao = userDao;
        }

        public async Task<User> AddUser(User user)
        {
            await _userDao.AddAsync(user);
            return user;
        }

        public async Task<User> GetUserById(string userId)
        {
            return await _userDao.GetByIdAsync(userId);
        }
    }
}
