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
    public interface IAsyncDao<T> where T : IEntity
    {
        Task<T> GetByIdAsync(string id);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
        Task<T> FirstOrDefaultAsync(ISpecification<T> spec);
        Task<T> AddAsync(T entity);
        Task AddAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateAsync(IEnumerable<T> entities);
        Task DeleteAsync(T entity);
        Task DeleteAsync(string id);
        Task DeleteAsync(IEnumerable<T> entities);
        Task DeleteAsync(IEnumerable<string> ids);
        Task DeleteAsync(ISpecification<T> spec);
        Task DeleteAsync(Expression<Func<T, bool>> criteria);
        Task<int> CountAsync(ISpecification<T> spec);
        Task<int> CountAsync(Expression<Func<T, bool>> criteria);
        Task<bool> AnyAsync(ISpecification<T> spec);
        Task<bool> AnyAsync(Expression<Func<T, bool>> criteria);

    }
}
