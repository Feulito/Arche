﻿using Core.Entities;
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
            await _userDao.AddAsync(user);
            return user;
        }

        public async Task<User> GetUserById(string userId)
        {
            return await _userDao.GetByIdAsync(userId);
        }

        public async Task<User> GetUserByNameAndMail(string userName, string email)
        {
            ISpecification<User> spec = new Specification<User>()
            {
                Criteria = u => !u.Deleted && u.UserName == userName && u.Email == email
            };
            return (await _userDao.ListAsync(spec)).FirstOrDefault();
        }

        public async Task<User> GetUserByMail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            ISpecification<User> spec = new Specification<User>()
            {
                Criteria = u => !u.Deleted &&  u.Email == email
            };
            return (await _userDao.ListAsync(spec)).FirstOrDefault();
        }

        private async Task CheckUserInfos(User user)
        {
            if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Email))
                throw new UserServiceException("Un utilisateur doit avoir un pseudo et un email.");
            if ((await GetUserByMail(user.Email)) != null)
                throw new UserServiceException("Il existe déjà un utilisateur avec cet email.");
        }

        public async Task<User> SignIn(string email, string pass)
        {
            User user = await GetUserByMail(email);
            if (user == null || !HasherUtility.CheckHash(pass, user.Password, user.Salt)) throw new AuthenticationException("Identifiants incorrects.");
            return user;
        }
    }
}
