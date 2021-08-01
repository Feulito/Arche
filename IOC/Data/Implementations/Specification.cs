using IOC.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IOC.Data.Implementations
{
    public class Specification<T> : ISpecification<T> where T : IEntity
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public Expression<Func<T, object>> OrderBy { get; set; }
        public bool IsOrderByDescending { get; set; }
        public Expression<Func<T, object>> GroupBy { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool IsPagingEnabled { get; set; }

        public List<Expression<Func<T, object>>> IncludeExpressions { get; set; }
        public List<string> IncludeStrings { get; set; }
    }
}
