using Autofac;
using Autofac.Core;
using AutoMapper;
using Hub.Infrastructure.Autofac;
using Hub.Infrastructure.Database;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.DependencyInjection;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using Hub.Infrastructure.Localization.Interfaces;
using Hub.Infrastructure.Mapper;
using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Configuration;

namespace Hub.Infrastructure
{
    public static class Engine
    {
        public static IContainer Container { get; set; }
        private static ContainerManager _containerManager;
        public static Assembly ExecutingAssembly { get; private set; }
        private static Action initializeAction = null;

        public static AsyncLocal<ILifetimeScope> CurrentScope = new AsyncLocal<ILifetimeScope>();

        private static ILocalizationProvider _localizationProvider;
        private static List<IAutoMapperStartup> _autoMapperStartups;


        class LifetimeScopeDispose : IDisposable
        {
            public ILifetimeScope Scope { get; set; }

            public bool IsDisposed { get; set; }

            public LifetimeScopeDispose(ILifetimeScope scope)
            {
                Scope = scope;
                IsDisposed = false;
            }

            public void Dispose()
            {
                if (CurrentScope.Value == Scope)
                {
                    CurrentScope.Value.Dispose();
                    CurrentScope.Value = null;
                }

                IsDisposed = true;
            }
        }

        public static void SetContainer(IContainer container)
        {
            _containerManager.Container = container;

            initializeAction();
        }

        public static void RunAutoMapperStartups(IMapperConfigurationExpression cfg)
        {
            if (_autoMapperStartups == null) return;

            foreach (var item in _autoMapperStartups)
            {
                item.RegisterMaps(cfg);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Initialize(Assembly executingAssembly, IList<IDependencyConfiguration> dependencyRegistrars = null, ContainerBuilder containerBuilder = null, ConnectionStringBaseVM csb = null)
        {
            ExecutingAssembly = executingAssembly;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            if (dependencyRegistrars == null)
            {
                dependencyRegistrars = new List<IDependencyConfiguration>();
            }

            dependencyRegistrars.Add(new DependencyRegistration());

            _containerManager = new ContainerManager(dependencyRegistrars, containerBuilder);

            initializeAction = new Action(() =>
            {
                if (_autoMapperStartups?.Count > 0)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        RunAutoMapperStartups(cfg);
                    });

                    Singleton<IMapper>.Instance = config.CreateMapper();
                }

                TryResolve(out _localizationProvider);

                if (csb != null)
                {
                    Resolve<ConnectionStringBaseConfigurator>().Set(csb);
                }

                IOrmConfiguration ormConfiguration = null;

                if (TryResolve(out ormConfiguration))
                {
                    ormConfiguration.Configure(csb);
                }
            });

            if (_containerManager.Container != null)
            {
                initializeAction();
            }
        }

        public static string ConnectionString(string settingName)
        {
            return ConfigurationManager.ConnectionStrings[settingName]?.ConnectionString;
        }

        #region RESOLVE 

        public static T Resolve<T>()
        {
            return ContainerManager.Resolve<T>("", CurrentScope.Value);
        }

        public static T Resolve<T>(ILifetimeScope scope)
        {
            return ContainerManager.Resolve<T>("", scope);
        }

        public static T Resolve<T>(Dictionary<string, object> parameters)
        {
            var listParameters = new List<Parameter>();

            foreach (var item in parameters)
            {
                listParameters.Add(new NamedParameter(item.Key, item.Value));
            }

            return ContainerManager.Container.Resolve<T>(listParameters);
        }

        public static object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        public static bool TryResolve<T>(out T instance) where T : class
        {
            return ContainerManager.Container.TryResolve(out instance);
        }

        public static bool TryResolve(Type type, out object instance)
        {
            return ContainerManager.Container.TryResolve(type, out instance);
        }

        public static object Resolve(Type type, params Type[] typeArguments)
        {
            return ContainerManager.Resolve(type.MakeGenericType(typeArguments));
        }

        public static bool TryResolve(Type type, out object instance, params Type[] typeArguments)
        {
            try
            {
                instance = ContainerManager.Resolve(type.MakeGenericType(typeArguments));

                return true;
            }
            catch (Exception)
            {
                instance = null;

                return false;
            }
        }

        public static T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

        public static string Get(string key)
        {
            if (_localizationProvider == null) return key;

            return _localizationProvider.Get(key);
        }

        public static string GetByValue(string value)
        {
            if (_localizationProvider == null) return value;

            return _localizationProvider.GetByValue(value);
        }

        public static string Get(string key, CultureInfo culture)
        {
            if (_localizationProvider == null) return key;

            return _localizationProvider.Get(key, culture);
        }

        public static string Get(string key, params object[] args)
        {
            if (_localizationProvider == null) return key;

            return string.Format(_localizationProvider.Get(key), args);
        }

        public static string Get(string key, CultureInfo culture, params object[] args)
        {
            if (_localizationProvider == null) return key;

            return string.Format(_localizationProvider.Get(key, culture), args);
        }

        public static ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion
    }
}
