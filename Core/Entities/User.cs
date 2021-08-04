using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class User : AbstractEntity
    {
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
