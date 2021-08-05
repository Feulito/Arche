using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.FormData
{
    public class LoginFormData
    {
        public string Email { get; set; }
        public string Pass { get; set; }
        public bool Persistent { get; set; }
    }
}
