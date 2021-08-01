using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IOC.Data.Interfaces
{
    public interface ISpecification<T> where T : IEntity
    {
        Expression<Func<T, bool>> Criteria { get; }
        Expression<Func<T, object>> OrderBy { get; }
        bool IsOrderByDescending { get; }
        Expression<Func<T, object>> GroupBy { get; }

        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }

        public List<Expression<Func<T, object>>> IncludeExpressions { get; }
        public List<string> IncludeStrings { get; }
    }
}
