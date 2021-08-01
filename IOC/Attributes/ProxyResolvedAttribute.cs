using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOC.Attributes
{
    public class ProxyResolvedAttribute : Attribute
    {

        public string IdPropertyName { get; }
        public bool AlreadyResolved { get; set; } = false;
        public ProxyResolvedAttribute(string idPropertyName)
        {
            IdPropertyName = idPropertyName;
        }

    }
}
