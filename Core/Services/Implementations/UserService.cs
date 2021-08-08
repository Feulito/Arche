using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Core.Services.Interfaces;
using Core.Utils;
using IOC.Data.Implementations;
using IOC.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
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
            await CheckUserInfos(user);
            User userDb = await GetUserByNameAndMail(user.UserName, user.Email);
            return await _userDao.AddAsync(user);
        }

        public async Task<User> GetUserById(string userId)
        {
            ISpecification<User> spec = new Specification<User>()
            {
                Criteria = u => !u.Deleted && u.Id == userId
            };
            return await _userDao.FirstOrDefaultAsync(spec);
        }

        public async Task<User> GetUserByNameAndMail(string userName, string email)
        {
            ISpecification<User> spec = new Specification<User>()
            {
                Criteria = u => !u.Deleted && u.UserName == userName && u.Email == email
            };
            return await _userDao.FirstOrDefaultAsync(spec);
        }

        public async Task<User> GetUserByMail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            ISpecification<User> spec = new Specification<User>()
            {
                Criteria = u => !u.Deleted &&  u.Email == email
            };
            return await _userDao.FirstOrDefaultAsync(spec);
        }

        private async Task CheckUserInfos(User user)
        {
            if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
                throw new UserServiceException("Un utilisateur doit avoir un pseudo, un Email et un mot de passe.");
            if ((await GetUserByMail(user.Email)) != null)
                throw new UserServiceException("Il existe déjà un utilisateur avec cet email.");
        }

        public async Task<User> SignIn(string email, string pass)
        {
            User user = await GetUserByMail(email);
            if (user == null || string.IsNullOrWhiteSpace(pass) || !HasherUtility.CheckHash(pass, user.Password, user.Salt)) throw new AuthenticationException("Identifiants incorrects.");
            return user;
        }

        public async Task DeleteById(string userId)
        {
            await Delete(await _userDao.GetByIdAsync(userId));
        }

        public async Task Delete(User user)
        {
            if (user.ProfileType == EProfileType.SuperAdmin) throw new UserServiceException("Impossible de supprimer un Super-Administrateur");
            user.Deleted = true;
            await _userDao.UpdateAsync(user);
        }
    }
}
