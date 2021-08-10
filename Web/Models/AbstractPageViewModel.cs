using Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class AbstractPageViewModel
    {
        public virtual string Title { get; }
        public string ErrorMessage { get; set; }
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public UserViewModel User { get; set; }
        public bool UserAuthenticated => User != null;
    }
}
