using Core.Entities;
using Core.Models;
using Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ArticlePageViewModel : AbstractPageViewModel
    {
        public override string Title => Article?.Title ?? "Article";
        public ArticleViewModel Article { get; set; }
        public User Auteur { get; set; }
    }
}
