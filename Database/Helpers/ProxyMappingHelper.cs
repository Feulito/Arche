using Castle.DynamicProxy;
using IOC.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Database.Helpers
{
    internal static class ProxyMappingHelper<T> where T : class, IEntity
    {
        private static readonly List<PropertyInfo> _properties = typeof(T).GetProperties().Where(p => !typeof(IEntity).IsAssignableFrom(p.PropertyType) && !typeof(IEnumerable<IEntity>).IsAssignableFrom(p.PropertyType)).ToList();
        private static readonly List<PropertyInfo> _entityProperties = typeof(T).GetProperties().Where(p => typeof(IEntity).IsAssignableFrom(p.PropertyType)).ToList();
        private static readonly List<PropertyInfo> _entityListProperties = typeof(T).GetProperties().Where(p => typeof(IEnumerable<IEntity>).IsAssignableFrom(p.PropertyType)).ToList();

        public static T Map(T parent, T child, bool mapEntities = false, EntityInterceptor interceptor = null)
        {
            foreach (PropertyInfo propertyInfo in _properties)
            {
                propertyInfo?.SetValue(child, propertyInfo.GetValue(parent));
            }

            if (!mapEntities) return child;

            if (parent.GetType().Assembly != Assembly.GetAssembly(typeof(IInterceptor)))
            {
                foreach (PropertyInfo propertyInfo in _entityProperties)
                {
                    object value = propertyInfo.GetValue(parent);
                    if (value != null)
                    {
                        value = typeof(ProxyMappingHelper<>).MakeGenericType(propertyInfo.PropertyType).GetMethod("CreateEntityProxy")?.Invoke(null, parameters: new object[] { value, interceptor });
                    }
                    propertyInfo?.SetValue(child, value);
                }

                foreach (PropertyInfo propertyInfo in _entityListProperties)
                {
                    object value = propertyInfo.GetValue(parent);
                    if (value != null)
                    {
                        value = typeof(ProxyMappingHelper<>).MakeGenericType(propertyInfo.PropertyType.GenericTypeArguments[0]).GetMethod("CreateEntityProxies")?.Invoke(null, parameters: new object[] { value, interceptor });
                    }
                    propertyInfo?.SetValue(child, value);
                }
            }
            else
            {
                foreach (PropertyInfo propertyInfo in _entityProperties)
                {
                    propertyInfo?.SetValue(child, propertyInfo.GetValue(parent));
                }

                foreach (PropertyInfo propertyInfo in _entityListProperties)
                {
                    propertyInfo?.SetValue(child, propertyInfo.GetValue(parent));
                }
            }

            return child;
        }
        public static async Task<T> CreateEntityProxyAsync(T entity, EntityInterceptor interceptor)
        {
            if (entity == null) return null;
            Task<T> task = new(() => CreateEntityProxy(entity, interceptor));
            task.Start();
            return await task;
        }
        public static async Task<IReadOnlyList<T>> CreateEntityProxiesAsync(IEnumerable<T> entities, EntityInterceptor interceptor)
        {
            List<T> entitiesList = entities.ToList();
            List<Task<T>> result = entitiesList.Select(entity => CreateEntityProxyAsync(entity, interceptor)).ToList();

            await Task.WhenAll(result);
            return result.Select(r => r.Result).ToList();
        }
        public static List<T> CreateEntityProxies(IEnumerable<T> entities, EntityInterceptor interceptor)
        {
            List<T> result = entities.Select(entity => CreateEntityProxy(entity, interceptor)).ToList();

            return result;
        }
        public static T CreateEntityProxy(T entity, EntityInterceptor interceptor)
        {
            if (entity == null) return null;

            string proxyId = $"{entity.Id}_{entity.GetType().Name}";

            if (interceptor.ProxyfiedElements.ContainsKey(proxyId))
                return interceptor.ProxyfiedElements[proxyId] as T;

            T proxy = interceptor.ProxyGenerator.CreateClassProxy<T>(interceptor);
            interceptor.ProxyfiedElements[proxyId] = proxy;
            Map(entity, proxy, true, interceptor);

            return proxy;

        }
    }
}
