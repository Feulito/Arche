using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IOC
{
    internal static class AssemblyHelper
    {
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException($"Can't find {nameof(assembly)} Assembly");
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        public static Type TryMakeGenericType(this Type type, params Type[] arguments)
        {
            try
            {
                return type.MakeGenericType(arguments);
            }
            catch
            {
                return null;
            }
        }
    }
}
