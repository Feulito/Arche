using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Site
{
    public class IndexViewModel
    {
        public string Title => "Arche - Accueil";
        public List<ArticleViewModel> Articles { get; set; } = new List<ArticleViewModel>();
    }
}
