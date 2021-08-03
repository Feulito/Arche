using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static class IEnumerableExtension
    {
        public static void ForEach<T>(this IEnumerable<T> current, Action<T> expression)
        {
            foreach (T t in current)
            {
                expression.Invoke(t);
            }
        }
    }
}
