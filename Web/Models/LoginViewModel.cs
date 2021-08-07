using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class LoginViewModel
    {
        public string Title => "Connexion";
        public string ErrorMessage { get; set; }
        public bool ConnexionFailed => !string.IsNullOrWhiteSpace(ErrorMessage);
    }
}
