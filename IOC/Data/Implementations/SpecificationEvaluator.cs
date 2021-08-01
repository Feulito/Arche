using IOC.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOC.Data.Implementations
{
    public static class SpecificationEvaluator<T> where T : IEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            IQueryable<T> query = inputQuery;

            IIncludeEvaluator<T> includeEvaluator = Container.Resolve<IIncludeEvaluator<T>>();
            if (includeEvaluator != null)
            {
                query = specification.IncludeExpressions?.Aggregate(query, (current, include) => includeEvaluator?.Include(current, include) ?? current) ?? query;
                query = specification.IncludeStrings?.Aggregate(query, (current, include) => includeEvaluator?.Include(current, include) ?? current) ?? query;
            }

            // modify the IQueryable using the specification's criteria expression
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Apply ordering if expressions is set
            if (specification.OrderBy != null)
            {
                query = specification.IsOrderByDescending ?
                    query.OrderByDescending(specification.OrderBy) : query.OrderBy(specification.OrderBy);
            }

            //Apply grouping if expression is set
            if (specification.GroupBy != null)
            {
                query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
            }

            // Apply paging if enabled
            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip)
                    .Take(specification.Take);
            }
            return query;
        }
    }
}
