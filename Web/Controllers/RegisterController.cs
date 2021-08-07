using Core.Entities;
using Core.Exceptions;
using Core.Models.FormData;
using Core.Services.Interfaces;
using Core.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IUserService _userService;

        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            RegisterViewModel viewModel = new RegisterViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Index(RegisterFormData registerFormData)
        {
            if (string.IsNullOrWhiteSpace(registerFormData.Pass)) return View(new RegisterViewModel() { ErrorMessage = "Veuillez entrer un mot de passe." });
            HashObject hashObject = HasherUtility.Hash(registerFormData.Pass);
            User user = new User()
            {
                UserName = registerFormData.UserName,
                Email = registerFormData.Email,
                Password =  hashObject.Hash,
                Salt = hashObject.Salt
            };
            try
            {
                user = await _userService.AddUser(user);
            } catch (UserServiceException e)
            {
                return View(new RegisterViewModel() { ErrorMessage = e.Message });
            }

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties authProperties = new()
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1),
                IsPersistent = false,
                IssuedUtc = DateTimeOffset.UtcNow,
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            return RedirectToActionPermanent("Index", "Index");
        }
    }
}
