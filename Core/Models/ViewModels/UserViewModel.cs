using Core.Enums;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public EProfileType ProfileType { get; set; }
        public string Role
        {
            get
            {
                return EnumUtility.GetEnumDescription(ProfileType);
            }
            set
            {
                return;
            }
        }

        public bool IsSuperAdmin()
        {
            return ProfileType.HasFlag(EProfileType.SuperAdmin);
        }

        public bool IsAtLeastAdmin()
        {
            return ProfileType.HasFlag(EProfileType.Admin) || IsSuperAdmin();
        }

        public bool IsAtLeastMJ()
        {
            return ProfileType.HasFlag(EProfileType.MJ) || IsAtLeastAdmin();
        }
    }
}
