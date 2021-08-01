using IOC.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IOC.Data.Interfaces
{
    [Resolvable]
    public interface IIncludeEvaluator<T> where T : IEntity
    {
        IQueryable<T> Include(IQueryable<T> query, Expression<Func<T, object>> include);

        IQueryable<T> Include(IQueryable<T> query, string include);
    }
}
