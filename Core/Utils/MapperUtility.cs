using Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utils
{
    public static class MapperUtility
    {
        public static Tto Map<Tfrom, Tto>(Tfrom from, Tto to)
        {
            foreach (PropertyInfo prop in to.GetType().GetProperties())
            {
                prop.SetValue(to, from.GetType().GetProperty(prop.Name)?.GetValue(from));
            }
            return to;
        }

        public static List<Tto> Map<Tfrom, Tto>(IEnumerable<Tfrom> fromList) where Tto : new()
        {
            List<Tto> list = new List<Tto>();
            fromList.ForEach(e => list.Add(Map(e, new Tto())));
            return list;
        }
    }
}
