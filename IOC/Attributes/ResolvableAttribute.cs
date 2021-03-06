using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOC.Attributes
{
    public class ResolvableAttribute : Attribute
    {
        public ServiceLifetime? ServiceLifetime = null;

        public ResolvableAttribute()
        {

        }

        public ResolvableAttribute(ServiceLifetime lifetime)
        {
            ServiceLifetime = lifetime;
        }

    }
}
