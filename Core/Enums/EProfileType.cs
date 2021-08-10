using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums
{
    [Flags]
    public enum EProfileType
    {
        [Description("Utilisateur")]
        User = 0,

        [Description("Maître du jeu")]
        MJ = 1,

        [Description("Administrateur")]
        Admin = 2,

        [Description("Super Administrateur")]
        SuperAdmin = 4,

        [Description("Support")]
        Support = 8,

        [Description("Developpeur")]
        Developper
    }
}
