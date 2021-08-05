using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.ViewModels
{
    public class ArticleViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string HeaderUrl { get; set; }
        public string Content { get; set; }
        public string AuteurId { get; set; }
    }
}
