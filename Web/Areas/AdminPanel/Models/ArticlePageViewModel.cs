using Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Areas.AdminPanel.Models
{
    public class ArticlePageViewModel
    {
        public string Title => "Panneau d'administration - Nouvel article";
        public UserViewModel User { get; set; }
        public bool UserAuthenticated => User != null;
    }
}
