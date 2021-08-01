using IOC.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Entities;

namespace Database
{
    public class IncludeEvaluator<T> : IIncludeEvaluator<T> where T: AbstractEntity
    {
        public IQueryable<T> Include(IQueryable<T> query, Expression<Func<T, object>> include)
        {
            return query.Include(include);
        }

        public IQueryable<T> Include(IQueryable<T> query, string include)
        {
            return query.Include(include);
        }
    }
}
