using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ProfileViewModel : AbstractPageViewModel
    {
        public override string Title => UserAuthenticated ? $"Profil - {User.UserName}" : "Profil";

    }
}
