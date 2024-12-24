using Autofac;
using Autofac.Core;
using AutoMapper;
using Hub.Infrastructure.Autofac;
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
using System.Collections.Specialized;
using Hub.Infrastructure.Extensions;

namespace Hub.Infrastructure
{
    public static class Engine
    {
        public static IContainer Container { get; set; }
        public static Assembly ExecutingAssembly { get; private set; }
        public static AsyncLocal<ILifetimeScope> CurrentScope = new AsyncLocal<ILifetimeScope>();
        //private static AsyncLocal<LifetimeScopeDispose> currentScopeDisposer = new AsyncLocal<LifetimeScopeDispose>();
        public static AsyncLocal<bool> IgnoreTenantConfigsScope = new AsyncLocal<bool>();
        private static ContainerManager _containerManager;
        private static ILocalizationProvider _localizationProvider;
        private static List<IAutoMapperStartup> _autoMapperStartups;
        private static object appSettingsLock = new object();

        //private static object appSettingsLock = new object();
        private static Action initializeAction = null;

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
        public static void Initialize(Assembly executingAssembly, IList<IDependencyConfiguration> dependencyRegistrars = null, ContainerBuilder containerBuilder = null)
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

            dependencyRegistrars.Add(new DependencyConfiguration());

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


                IOrmConfiguration ormConfiguration = null;

                if (TryResolve(out ormConfiguration))
                {
                    ormConfiguration.Configure();
                }
            });

            if (_containerManager.Container != null)
            {
                initializeAction();
            }
        }

        public static string ConnectionString(string settingName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[settingName];

            if (connectionString != null)
                return connectionString.ConnectionString;
            else
            {
                var key = AppSettings[$"ConnectionString-{settingName}"];

                if (key != null)
                {
                    return key;
                }

                return Environment.GetEnvironmentVariable($"ConnectionString-{settingName}");
            }
        }

        static NameValueCollection ConfigCollection = null;

        static Dictionary<string, NameValueCollection> TenantConfigCollection = new Dictionary<string, NameValueCollection>();

        static AsyncLocal<NameValueCollection> AsyncLocalConfigCollection = new AsyncLocal<NameValueCollection>();

        public static NameValueCollection AppSettings
        {
            get
            {
                if (ConfigCollection == null)
                {
                    lock (appSettingsLock)
                    {
                        if (ConfigCollection == null)
                        {
                            NameValueCollection all;

                            if (ConfigurationManager.AppSettings.Keys.Count != 0)
                            {
                                all = new NameValueCollection(ConfigurationManager.AppSettings);
                            }
                            else
                            {
                                all = new NameValueCollection(Environment.GetEnvironmentVariables().ToNameValueCollection());
                            }

                            // Aplicar chaves de debug (substituir qualquer chave quando em modo Debug)
                            // Exemplo de chave no appsettings.json: "Debug:elos-api-endpoint"
                            foreach (var debugKey in all.AllKeys.Where(a => a.StartsWith("Debug:")))
                            {
                                var originalKey = debugKey.Replace("Debug:", "");

                                if (all.AllKeys.Contains(originalKey))
                                {
                                    all[originalKey] = all[debugKey];
                                }
                                else
                                {
                                    all.Add(originalKey, all[debugKey]);
                                }
                            }

                            ConfigCollection = all;
                        }
                    }
                }

                string tenantName = "";

                //if (ContainerManager?.Container != null && IgnoreTenantConfigsScope.Value == false)
                //{
                //    tenantName = Singleton<ISchemaNameProvider>.Instance.TenantName();

                //    if (tenantName.Equals("system", StringComparison.OrdinalIgnoreCase))
                //    {
                //        tenantName = "";
                //    }
                //}

                if (!string.IsNullOrEmpty(tenantName))
                {
                    if (!TenantConfigCollection.ContainsKey(tenantName))
                    {
                        lock (appSettingsLock)
                        {
                            if (!TenantConfigCollection.ContainsKey(tenantName))
                            {
                                // Carregar configurações específicas do tenant a partir do appsettings
                                var tenantPrefix = $"{tenantName}:";
                                var tenantSettings = ConfigurationManager.AppSettings
                                    .AllKeys
                                    .Where(k => k.StartsWith(tenantPrefix, StringComparison.OrdinalIgnoreCase))
                                    .ToDictionary(
                                        k => k.Substring(tenantPrefix.Length),
                                        k => ConfigurationManager.AppSettings[k]
                                    );

                                var all = new NameValueCollection(ConfigCollection);

                                foreach (var kvp in tenantSettings)
                                {
                                    all[kvp.Key] = kvp.Value;
                                }

                                // Aplicar chaves de debug para o tenant
                                foreach (var debugKey in all.AllKeys.Where(a => a.StartsWith("Debug:")))
                                {
                                    var originalKey = debugKey.Replace("Debug:", "");

                                    if (all.AllKeys.Contains(originalKey))
                                    {
                                        all[originalKey] = all[debugKey];
                                    }
                                    else
                                    {
                                        all.Add(originalKey, all[debugKey]);
                                    }
                                }

                                TenantConfigCollection.Add(tenantName, all);
                            }
                        }
                    }

                    if (AsyncLocalConfigCollection.Value != null)
                    {
                        var all = new NameValueCollection(TenantConfigCollection[tenantName]);

                        foreach (string key in AsyncLocalConfigCollection.Value)
                        {
                            all[key] = AsyncLocalConfigCollection.Value[key];
                        }

                        return all;
                    }

                    return TenantConfigCollection[tenantName];
                }

                if (AsyncLocalConfigCollection.Value != null)
                {
                    var all = new NameValueCollection(ConfigCollection);

                    foreach (string key in AsyncLocalConfigCollection.Value)
                    {
                        all[key] = AsyncLocalConfigCollection.Value[key];
                    }

                    return all;
                }

                return ConfigCollection;
            }
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
