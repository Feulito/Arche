using Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class IndexViewModel : AbstractPageViewModel
    {
        public override string Title => "Arche - Accueil";
        public List<ArticleViewModel> Articles { get; set; } = new List<ArticleViewModel>();
    }
}
