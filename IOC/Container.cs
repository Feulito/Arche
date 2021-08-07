using IOC.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IOC
{
    public static class Container
    {
        private static IServiceCollection _container;
        private static IServiceProvider _provider;

        /// <summary>
        /// Get or set Service collection used as _container.
        /// </summary>
        public static IServiceCollection ServiceCollection
        {
            get => _container ??= new ServiceCollection();
            set
            {
                Provider = null;
                _container = value;
            }
        }

        /// <summary>
        /// Get or set the service provider.
        /// </summary>
        public static IServiceProvider Provider
        {
            get => _provider ??= ServiceCollection.BuildServiceProvider();
            set => _provider = value;
        }


        private static Assembly[] _interfacesAssemblies;
        public static Assembly[] InterfacesAssemblies
        {
            get => _interfacesAssemblies ??= AppDomain.CurrentDomain.GetAssemblies();
            set => _interfacesAssemblies = value;
        }


        private static Assembly[] _implementationsAssemblies;
        public static Assembly[] ImplementationsAssemblies
        {
            get => _implementationsAssemblies ??= AppDomain.CurrentDomain.GetAssemblies();
            set => _implementationsAssemblies = value;
        }




        /// <summary>
        /// Resolve existing dependency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return Provider.GetService<T>();
        }

        /// <summary>
        /// Resolve an existing dependency
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Resolve(Type type)
        {
            return Provider.GetService(type);
        }

        /// <summary>
        /// Register all dependencies with [IocResolved] attribute
        /// </summary>
        public static void RegisterAllTypes(ServiceLifetime defaultLifetime = ServiceLifetime.Singleton)
        {
            foreach (Assembly a in InterfacesAssemblies)
            {
                IEnumerable<Type> types = a.GetLoadableTypes().Where(t => t.GetCustomAttributes(typeof(ResolvableAttribute), true).Any());
                foreach (Type t in types)
                {

                    ResolvableAttribute resolvableAttribute = t.GetCustomAttributes(typeof(ResolvableAttribute), true).First() as ResolvableAttribute;
                    ServiceLifetime lifetime = resolvableAttribute?.ServiceLifetime ?? defaultLifetime;

                    if (t.IsGenericType)
                    {
                        RegisterGenericType(t, lifetime);
                    }
                    else
                    {
                        RegisterImplementation(t, lifetime, SearchNonGenericImplementation(t));
                    }
                }
            }
        }

        /// <summary>
        /// Register all constructed types derived from generic type.
        /// Only support generic types with one generic argument !
        /// </summary>
        /// <param name="type"></param>
        /// <param name="lifetime"></param>
        private static void RegisterGenericType(Type type, ServiceLifetime lifetime)
        {
            if (type.GetGenericArguments().Count() != 1)
            {
                throw new ArgumentException($"Only GenericTypes with one generic argument are supported ({type.Name})");
            }

            Type argumentDefinition = type.GetGenericArguments()[0].GetGenericParameterConstraints()[0];

            List<Type> foundArguments = ImplementationsAssemblies.SelectMany(s => s.GetLoadableTypes()).Where(p => !p.IsGenericType && !p.IsInterface && argumentDefinition.IsAssignableFrom(p)).ToList();
            foreach (Type t in foundArguments)
            {
                Type constructedGenericType = type.MakeGenericType(t);
                Type implementation = SearchNonGenericImplementation(constructedGenericType) ?? SearchGenericImplementation(constructedGenericType, t);
                if (implementation != null)
                {
                    RegisterImplementation(constructedGenericType, lifetime, implementation);
                }
            }
        }

        /// <summary>
        ///  Search and return a concrete generic implementation of genericType in all available assemblies.
        /// </summary>
        /// <param name="genericType"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        private static Type SearchGenericImplementation(Type genericType, Type argument)
        {
            Type implementation = ImplementationsAssemblies.SelectMany(s => s.GetLoadableTypes())
                                                            .FirstOrDefault(p => p.IsGenericType && !p.IsInterface && !p.IsAbstract && genericType.IsAssignableFrom(p.TryMakeGenericType(argument)));
            implementation = implementation?.MakeGenericType(argument);
            return implementation;
        }

        /// <summary>
        /// Search and return a non generic concrete implementation of type in all available assemblies.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Type SearchNonGenericImplementation(Type type)
        {
            return ImplementationsAssemblies.SelectMany(s => s.GetLoadableTypes())
                                            .FirstOrDefault(p => !p.IsGenericType && !p.IsInterface && !p.IsAbstract && type.IsAssignableFrom(p));
        }

        /// <summary>
        /// Register concrete implementation of type in the service collection.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="lifetime"></param>
        /// <param name="implementation"></param>
        private static void RegisterImplementation(Type type, ServiceLifetime lifetime, Type implementation)
        {
            if (implementation == null)
            {
                throw new ArgumentException($"{type.Name} cannot find or load it's implementation");
            }

            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    ServiceCollection.AddScoped(type, implementation);
                    break;
                case ServiceLifetime.Singleton:
                    ServiceCollection.AddSingleton(type, implementation);
                    break;
                case ServiceLifetime.Transient:
                    ServiceCollection.AddTransient(type, implementation);
                    break;

            }
        }

    }
}
