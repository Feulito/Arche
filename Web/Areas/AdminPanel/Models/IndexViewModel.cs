using Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Areas.AdminPanel.Models
{
    public class IndexViewModel
    {
        public string Title => "Panneau d'administration - Accueil";
        public UserViewModel User { get; set; }
        public bool UserAuthenticated => User != null;
    }
}
