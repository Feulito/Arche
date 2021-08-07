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
    internal static class ProxyMappingHelper<TM> where TM : class, IEntity
    {
        private static readonly List<PropertyInfo> _properties = typeof(TM).GetProperties().Where(p => !typeof(IEntity).IsAssignableFrom(p.PropertyType) && !typeof(IEnumerable<IEntity>).IsAssignableFrom(p.PropertyType)).ToList();
        private static readonly List<PropertyInfo> _entityProperties = typeof(TM).GetProperties().Where(p => typeof(IEntity).IsAssignableFrom(p.PropertyType)).ToList();
        private static readonly List<PropertyInfo> _entityListProperties = typeof(TM).GetProperties().Where(p => typeof(IEnumerable<IEntity>).IsAssignableFrom(p.PropertyType)).ToList();

        public static TM Map(Type type, TM parent, TM child, bool mapEntities = false, EntityInterceptor interceptor = null)
        {
            List<PropertyInfo> properties = type.GetProperties().Where(p => !typeof(IEntity).IsAssignableFrom(p.PropertyType) && !typeof(IEnumerable<IEntity>).IsAssignableFrom(p.PropertyType)).ToList();
            List<PropertyInfo> entityProperties = type.GetProperties().Where(p => typeof(IEntity).IsAssignableFrom(p.PropertyType)).ToList();
            List<PropertyInfo> entityListProperties = type.GetProperties().Where(p => typeof(IEnumerable<IEntity>).IsAssignableFrom(p.PropertyType)).ToList();


            foreach (PropertyInfo propertyInfo in properties)
            {
                propertyInfo?.SetValue(child, propertyInfo.GetValue(parent));
            }

            if (!mapEntities) return child;

            if (parent.GetType().Assembly != Assembly.GetAssembly(typeof(IInterceptor)))
            {
                foreach (PropertyInfo propertyInfo in entityProperties)
                {
                    object value = propertyInfo.GetValue(parent);

                    if (value != null)
                    {
                        value = typeof(ProxyMappingHelper<>).MakeGenericType(propertyInfo.PropertyType).GetMethod("CreateEntityProxy")?.Invoke(null, parameters: new object[] { value, interceptor });
                    }
                    propertyInfo?.SetValue(child, value);
                }

                foreach (PropertyInfo propertyInfo in entityListProperties)
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
                foreach (PropertyInfo propertyInfo in entityProperties)
                {
                    propertyInfo?.SetValue(child, propertyInfo.GetValue(parent));
                }

                foreach (PropertyInfo propertyInfo in entityListProperties)
                {
                    propertyInfo?.SetValue(child, propertyInfo.GetValue(parent));
                }
            }

            return child;
        }
        public static TM Map(TM parent, TM child, bool mapEntities = false, EntityInterceptor interceptor = null)
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
        public static async Task<TM> CreateEntityProxyAsync(TM entity, EntityInterceptor interceptor)
        {
            if (entity == null) return null;
            Task<TM> task = new(() => CreateEntityProxy(entity, interceptor));
            task.Start();
            return await task;
        }
        public static async Task<IReadOnlyList<TM>> CreateEntityProxiesAsync(IEnumerable<TM> entities, EntityInterceptor interceptor)
        {
            List<TM> entitiesList = entities.ToList();
            List<Task<TM>> result = entitiesList.Select(entity => CreateEntityProxyAsync(entity, interceptor)).ToList();

            await Task.WhenAll(result);
            return result.Select(r => r.Result).ToList();
        }
        public static List<TM> CreateEntityProxies(IEnumerable<TM> entities, EntityInterceptor interceptor)
        {
            List<TM> result = entities.Select(entity => CreateEntityProxy(entity, interceptor)).ToList();

            return result;
        }
        public static TM CreateEntityProxy(TM entity, EntityInterceptor interceptor)
        {

            Type t = entity?.GetType() ?? typeof(TM);

            string proxyId = $"{entity.Id}_{t.Name}";

            if (entity is IProxyTargetAccessor)
            {
                interceptor.ProxyfiedElements[proxyId] = entity;
            }

            if (interceptor.ProxyfiedElements.ContainsKey(proxyId))
                return interceptor.ProxyfiedElements[proxyId] as TM;


            MethodInfo createClassProxy = typeof(ProxyGenerator).GetMethod("CreateClassProxy", new Type[] { typeof(IInterceptor[]) });

            TM proxy = createClassProxy.MakeGenericMethod(new Type[] { t }).Invoke(interceptor.ProxyGenerator, new object[] { new IInterceptor[] { interceptor } }) as TM;

            interceptor.ProxyfiedElements[proxyId] = proxy;
            Type baseType = t.BaseType;

            while (baseType != null && !baseType.IsAbstract)
            {
                interceptor.ProxyfiedElements[$"{entity.Id}_{baseType.Name}"] = proxy;
                baseType = baseType.BaseType;
            }
            Map(t, entity, proxy, true, interceptor);

            return proxy;

        }
    }

}
