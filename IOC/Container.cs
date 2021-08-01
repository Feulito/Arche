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
        /// Obtient ou définit la collection de services
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
        /// Obtient ou définit le service provider
        /// </summary>
        public static IServiceProvider Provider
        {
            get => _provider ??= ServiceCollection.BuildServiceProvider();
            set => _provider = value;
        }



        /// <summary>
        /// Réduit la dépendance existante de type T
        /// </summary>
        /// <typeparam name="T">Type de dépendance</typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return Provider.GetService<T>();
        }

        /// <summary>
        /// Réduit la dépendance existante d'un type donné
        /// </summary>
        /// <param name="type">Type de la dépendance</param>
        /// <returns></returns>
        public static object Resolve(Type type)
        {
            return Provider.GetService(type);
        }

        /// <summary>
        /// Enregistre tous les types avec l'attribut [Resolvable]
        /// </summary>
        public static void RegisterAllTypes(ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
            {
                IEnumerable<Type> types = a.GetLoadableTypes().Where(t => t.GetCustomAttributes(typeof(ResolvableAttribute), true).Any());
                foreach (Type t in types)
                {
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
        /// Enregistre tous les type dérivés d'un type générique
        /// Les types générique n'acceptent qu'un argument générique
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

            List<Type> foundArguments = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetLoadableTypes()).Where(p => !p.IsGenericType && !p.IsInterface && argumentDefinition.IsAssignableFrom(p)).ToList();
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
        ///  Retourne une impléméntation d'un type générique disponible dans les assemblies disponibles
        /// </summary>
        /// <param name="genericType"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        private static Type SearchGenericImplementation(Type genericType, Type argument)
        {
            Type implementation = AppDomain.CurrentDomain.GetAssemblies()
                                                    .SelectMany(s => s.GetLoadableTypes())
                                                    .FirstOrDefault(p => p.IsGenericType && !p.IsInterface && !p.IsAbstract && genericType.IsAssignableFrom(p.TryMakeGenericType(argument)));
            implementation = implementation?.MakeGenericType(argument);
            return implementation;
        }

        /// <summary>
        /// Retourne une implémentation concrete d'un type non générique disponible dans toutes les assemblies
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Type SearchNonGenericImplementation(Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                                                    .SelectMany(s => s.GetLoadableTypes())
                                                    .FirstOrDefault(p => !p.IsGenericType && !p.IsInterface && !p.IsAbstract && type.IsAssignableFrom(p));
        }

        /// <summary>
        /// Enregistre l'implémentation du type donné dans la collection de service
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
