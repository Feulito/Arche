using Castle.DynamicProxy;
using Database.Dao;
using IOC;
using IOC.Attributes;
using IOC.Data.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Database.Helpers
{
    internal class EntityInterceptor : IInterceptor
    {
        public readonly ProxyGenerator ProxyGenerator = new();
        public IDictionary<string, IEntity> ProxyfiedElements = new ConcurrentDictionary<string, IEntity>();
        public void Intercept(IInvocation invocation)
        {
            if (ShouldInterceptMethod(invocation))
            {
                PropertyInfo property = invocation.TargetType.GetProperty(invocation.Method.Name[4..]);
                if (property != null)
                {
                    ProxyResolvedAttribute proxyResolvedAttribute = GetProxyResolvedAttribute(property);
                    string id = invocation.TargetType.GetProperty(proxyResolvedAttribute.IdPropertyName)
                        ?.GetValue(invocation.InvocationTarget) as string;
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        string proxyId = $"{id}_{property.PropertyType.Name}";
                        if (!ProxyfiedElements.ContainsKey(proxyId))
                        {
                            object value = FindById(property.PropertyType, id);
                            value = typeof(ProxyMappingHelper<>).MakeGenericType(property.PropertyType)
                                .GetMethod("CreateEntityProxy")?.Invoke(null, parameters: new object[] { value, this });

                            property.SetValue(invocation.InvocationTarget, value);
                        }
                        else
                        {
                            property.SetValue(invocation.InvocationTarget, ProxyfiedElements[proxyId]);
                        }
                    }

                }
            }
            invocation.Proceed();
        }
        private static bool ShouldInterceptMethod(IInvocation invocation)
        {
            return invocation.Method.Name.StartsWith("get_", StringComparison.Ordinal)
                   && typeof(IEntity).IsAssignableFrom(invocation.Method.ReturnType);
        }
        public static ProxyResolvedAttribute GetProxyResolvedAttribute(MemberInfo prop)
        {
            return prop?.GetCustomAttributes<ProxyResolvedAttribute>().First();
        }
        private static object FindById(Type t, string id)
        {

            object dao = Container.Resolve(typeof(AbstractDao<>).MakeGenericType(t));
            object task = dao.GetType().GetMethod("GetByIdAsync")?.Invoke(dao, parameters: new object[] { id });
            return task?.GetType().GetProperty("Result")?.GetValue(task);
        }

    }
}
