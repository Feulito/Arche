using Database.Helpers;
using IOC.Data.Implementations;
using IOC.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Dao
{
    public abstract class AbstractDao<T> : IAsyncDao<T> where T : class, IEntity, new()
    {
        protected readonly ArcheDbContext _dbContext;

        private static EntityInterceptor Interceptor => new();

        public AbstractDao(ArcheDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> AddAsync(T entity)
        {
            T dbEntity = new T();
            ProxyMappingHelper<T>.Map(entity, dbEntity);
            await _dbContext.Set<T>().AddAsync(dbEntity);
            await _dbContext.SaveChangesAsync();
            return await ProxyMappingHelper<T>.CreateEntityProxyAsync(dbEntity, Interceptor);
        }

        public async Task AddAsync(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                T dbEntity = new T();
                ProxyMappingHelper<T>.Map(entity, dbEntity);
                await _dbContext.Set<T>().AddAsync(dbEntity);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(ISpecification<T> spec)
        {
            await DeleteAsync(await ListAsync(spec));
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        public async Task UpdateAsync(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                T dbEntity = await _dbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == entity.Id);
                entity.Update = DateTime.Now;
                ProxyMappingHelper<T>.Map(entity, dbEntity);
                EntityEntry<T> entry = _dbContext.Entry(dbEntity);
                entry.State = EntityState.Modified;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            await DeleteAsync(entity.Id);
        }
        public async Task DeleteAsync(IEnumerable<T> entities)
        {
            await DeleteAsync(entities.Select(e => e.Id));
        }

        public async Task DeleteAsync(string id)
        {
            T dbEntity = await _dbContext.FindAsync<T>(id);
            _dbContext.Set<T>().Remove(dbEntity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(IEnumerable<string> ids)
        {
            foreach (string id in ids)
            {
                T dbEntity = await _dbContext.FindAsync<T>(id);
                _dbContext.Set<T>().Remove(dbEntity);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await ProxyMappingHelper<T>.CreateEntityProxyAsync(await _dbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id), Interceptor);
        }

        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await ProxyMappingHelper<T>.CreateEntityProxiesAsync(await _dbContext.Set<T>().ToListAsync(), Interceptor);
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            return await ProxyMappingHelper<T>.CreateEntityProxiesAsync(await ApplySpecification(spec).ToListAsync(), Interceptor);
        }

        public async Task UpdateAsync(T entity)
        {
            T dbEntity = await _dbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == entity.Id);
            entity.Update = DateTime.Now;
            ProxyMappingHelper<T>.Map(entity, dbEntity);
            EntityEntry<T> entry = _dbContext.Entry(dbEntity);
            entry.State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        protected IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
        }
    }
}
