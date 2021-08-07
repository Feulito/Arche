using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class RegisterViewModel
    {
        public string Title = "Arche - S'inscrire";
        public string ErrorMessage { get; set; }
        public bool RegisterationFailed => !string.IsNullOrWhiteSpace(ErrorMessage);
    }
}
