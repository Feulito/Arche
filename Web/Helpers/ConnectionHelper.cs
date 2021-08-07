using Core.Entities;
using Core.Models.ViewModels;
using Core.Services.Interfaces;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Helpers
{
    public static class ConnectionHelper
    {
        public static async Task<UserViewModel> GetRights(ClaimsPrincipal OidcUser, IUserService usersService)
        {
            string id = OidcUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null) return null;

            User user = await usersService.GetUserById(id);
            if (user == null) return null;
            UserViewModel userViewModel = MapperUtility.Map(user, new UserViewModel());
            return userViewModel;
        }

    }
}
