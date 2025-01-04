#region TESTE - REFATORAÇÃO 

//using Autofac;
//using System.Configuration;
//using System.Collections.Specialized;
//using Hub.Infrastructure.Extensions;
//using AutoMapper;
//using Microsoft.EntityFrameworkCore;
//using Hub.Infrastructure.DependencyInjection.Interfaces;
//using System.Reflection;
//using System.Runtime.CompilerServices;
//using Newtonsoft.Json;
//using Hub.Infrastructure.DependencyInjection;
//using Hub.Infrastructure.Autofac;
//using Hub.Infrastructure.Database.Interfaces;
//using System.Globalization;
//using Autofac.Core;
//using Hub.Infrastructure.Helpers;
//using Hub.Infrastructure.Database;
//using Hub.Infrastructure.Architecture.Localization.Interfaces;
//using Hub.Infrastructure.Architecture.Mapper;
//using Hub.Infrastructure.Architecture.Tasks.Interfaces;
//using System.Threading.Tasks;
//using Hub.Infrastructure.Architecture.Tasks;

//namespace Hub.Infrastructure.Architecture
//{
//    public static class Engine
//    {
//        #region APP SETTINGS 

//        /// <summary>
//        /// Permite substituir/criar uma configuração para o escopo atual
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="value"></param>
//        public static void SetThreadSetting(string key, string value)
//        {
//            if (AsyncLocalConfigCollection.Value == null)
//            {
//                AsyncLocalConfigCollection.Value = new NameValueCollection();
//            }

//            if (AsyncLocalConfigCollection.Value.AllKeys.Any(c => c == key))
//            {
//                AsyncLocalConfigCollection.Value[key] = value;
//            }
//            else
//            {
//                AsyncLocalConfigCollection.Value.Add(key, value);
//            }
//        }

//        public static void RemoveThreadSetting(string key)
//        {
//            if (AsyncLocalConfigCollection.Value == null)
//            {
//                return;
//            }

//            if (AsyncLocalConfigCollection.Value.AllKeys.Any(c => c == key))
//            {
//                AsyncLocalConfigCollection.Value.Remove(key);
//            }
//        }

//        public class ScopeAppSetting : IDisposable
//        {
//            private readonly string key;
//            private readonly string originalValue;

//            public ScopeAppSetting(string key, string originalValue)
//            {
//                this.key = key;
//                this.originalValue = originalValue;
//            }

//            public void Dispose()
//            {
//                SetThreadSetting(key, originalValue);
//            }
//        }
//        public static IDisposable SetScopedSetting(string key, string value)
//        {
//            var currentValue = AppSettings[key];

//            SetThreadSetting(key, value);

//            return new ScopeAppSetting(key, currentValue);
//        }

//        static NameValueCollection ConfigCollection = null;

//        static Dictionary<string, NameValueCollection> TenantConfigCollection = new Dictionary<string, NameValueCollection>();

//        static AsyncLocal<NameValueCollection> AsyncLocalConfigCollection = new AsyncLocal<NameValueCollection>();

//        private static object appSettingsLock = new object();

//        public static NameValueCollection AppSettings
//        {
//            get
//            {
//                if (ConfigCollection == null)
//                {
//                    lock (appSettingsLock)
//                    {
//                        if (ConfigCollection == null)
//                        {
//                            NameValueCollection all = null;

//                            if (ConfigurationManager.AppSettings.Keys.Count != 0)
//                            {
//                                all = new NameValueCollection(ConfigurationManager.AppSettings);
//                            }
//                            else
//                            {
//                                all = new NameValueCollection(Environment.GetEnvironmentVariables().ToNameValueCollection());
//                            }


//                            //chaves de debug (usadas para substituir qualquer chave quando em modo Debug)
//                            //exemplo de chave no local.settings.json: "Debug:elos-api-endpoint"
//                            foreach (var debugKey in all.AllKeys.Where(a => a.StartsWith("Debug:")))
//                            {
//                                var originalKey = debugKey.Replace("Debug:", "");

//                                if (all.AllKeys.Contains(originalKey))
//                                {
//                                    all[originalKey] = all[debugKey];
//                                }
//                                else
//                                {
//                                    all.Add(originalKey, all[debugKey]);
//                                }
//                            }

//                            ConfigCollection = all;
//                        }
//                    }
//                }

//                if (AsyncLocalConfigCollection.Value != null)
//                {
//                    var all = new NameValueCollection(ConfigCollection);

//                    foreach (string key in AsyncLocalConfigCollection.Value)
//                    {
//                        all[key] = AsyncLocalConfigCollection.Value[key];
//                    }

//                    return all;
//                }

//                return ConfigCollection;
//            }
//        }

//        #endregion

//        #region CONNECTION STRINGS 

//        public static string ConnectionString(string settingName)
//        {
//            var cs = ConfigurationManager.ConnectionStrings[settingName];

//            if (cs != null)
//                return cs.ConnectionString;
//            else
//            {
//                var key = AppSettings[$"ConnectionString-{settingName}"];

//                if (key != null)
//                {
//                    return key;
//                }

//                return Environment.GetEnvironmentVariable($"ConnectionString-{settingName}");
//            }
//        }

//        #endregion

//        #region MAPPER 

//        private static List<IAutoMapperStartup> _autoMapperStartups;

//        public static void RunAutoMapperStartups(IMapperConfigurationExpression cfg)
//        {
//            if (_autoMapperStartups == null) return;

//            foreach (var item in _autoMapperStartups)
//            {
//                item.RegisterMaps(cfg);
//            }
//        }

//        public static void RegisterAutoMapperStartup(IAutoMapperStartup register)
//        {
//            if (_autoMapperStartups == null) _autoMapperStartups = new List<IAutoMapperStartup>();

//            _autoMapperStartups.Add(register);
//        }


//        #endregion

//        public static Assembly ExecutingAssembly { get; private set; }
//        private static ContainerManager _containerManager;
//        private static Action initializeAction = null;
//        private static ILocalizationProvider _localizationProvider;
//        public static AsyncLocal<ILifetimeScope> CurrentScope = new AsyncLocal<ILifetimeScope>();
//        private static AsyncLocal<LifetimeScopeDispose> currentScopeDisposer = new AsyncLocal<LifetimeScopeDispose>();

//        class LifetimeScopeDispose : IDisposable
//        {
//            public ILifetimeScope Scope { get; set; }

//            public bool IsDisposed { get; set; }

//            public LifetimeScopeDispose(ILifetimeScope scope)
//            {
//                Scope = scope;
//                IsDisposed = false;
//            }

//            public void Dispose()
//            {
//                if (CurrentScope.Value == Scope)
//                {
//                    CurrentScope.Value.Dispose();
//                    CurrentScope.Value = null;
//                }

//                IsDisposed = true;
//            }
//        }

//        public static void SetContainer(IContainer container)
//        {
//            _containerManager.Container = container;
//            initializeAction();
//        }

//        [MethodImpl(MethodImplOptions.Synchronized)]
//        public static void Initialize(EngineInitializationParameters engineInitializationParameters)
//        {
//            Initialize(executingAssembly: engineInitializationParameters.ExecutingAssembly, nameProvider: engineInitializationParameters.NameProvider, tasks: engineInitializationParameters.StartupTasks, dependencyRegistrars: engineInitializationParameters.DependencyRegistrators, csb: engineInitializationParameters.ConnectionStringBase, containerBuilder: engineInitializationParameters.ContainerBuilder);
//        }

//        [MethodImpl(MethodImplOptions.Synchronized)]
//        public static void Initialize(Assembly executingAssembly, IEntityNameProvider nameProvider = null, IList<IStartupTask> tasks = null, IList<IDependencyConfiguration> dependencyRegistrars = null, ConnectionStringBaseVM csb = null, ContainerBuilder containerBuilder = null)
//        {
//            ExecutingAssembly = executingAssembly;

//            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
//            {
//                TypeNameHandling = TypeNameHandling.Auto
//            };

//            if (nameProvider == null)
//            {
//                nameProvider = new DefaultNameProvider();
//            }

//            Singleton<IEntityNameProvider>.Instance = nameProvider;

//            Singleton<LoopTenantManager>.Instance = new LoopTenantManager();

//            if (dependencyRegistrars == null)
//            {
//                dependencyRegistrars = new List<IDependencyConfiguration>();
//            }

//            dependencyRegistrars.Add(new DependencyConfiguration());

//            _containerManager = new ContainerManager(dependencyRegistrars, containerBuilder);

//            initializeAction = new Action(() =>
//            {
//                if (tasks == null)
//                {
//                    tasks = new List<IStartupTask>();
//                }

//                tasks.Add(new StartupTask());

//                //startup tasks
//                tasks.OrderBy(t => t.Order).ToList().ForEach(x => x.Execute());

//                if (_autoMapperStartups?.Count > 0)
//                {
//                    var config = new MapperConfiguration(cfg =>
//                    {
//                        RunAutoMapperStartups(cfg);
//                    });

//                    Singleton<IMapper>.Instance = config.CreateMapper();
//                }

//                TryResolve<ILocalizationProvider>(out _localizationProvider);

//                if (csb != null)
//                {
//                    Resolve<ConnectionStringBaseConfigurator>().Set(csb);
//                }

//                IOrmConfiguration ormConfiguration = null;

//                if (TryResolve(out ormConfiguration))
//                {
//                    ormConfiguration.Configure();
//                }
//            });

//            if (_containerManager.Container != null)
//            {
//                initializeAction();
//            }
//        }

//        /// <summary>
//        /// Inicia um novo ciclo de vida do injetor de dependências
//        /// </summary>
//        /// <param name="preserveTenantScope">define se irá executar o comando que passa o nome do tenant atual para o novo ciclo criado</param>
//        /// <returns></returns>
//        public static IDisposable BeginLifetimeScope(bool preserveTenantScope = false)
//        {
//            if (currentScopeDisposer.Value == null || currentScopeDisposer.Value.IsDisposed)
//            {
//                string tenantName = null;

//                if (preserveTenantScope)
//                {
//                    tenantName = Singleton<IEntityNameProvider>.Instance.TenantName();
//                }

//                Engine.CurrentScope.Value = Engine.ContainerManager.Container.BeginLifetimeScope();

//                currentScopeDisposer.Value = new LifetimeScopeDispose(Engine.CurrentScope.Value);

//                if (preserveTenantScope)
//                {
//                    Engine.Resolve<TenantLifeTimeScope>().Start(tenantName);
//                }

//                return currentScopeDisposer.Value;
//            }

//            return null;
//        }

//        /// <summary>
//        /// Inicia um novo ciclo de vida do injetor de dependências
//        /// </summary>
//        /// <param name="tenantName">nome do tenant para o ciclo de vida que será criado</param>
//        /// <param name="applyTenantSettings"></param>
//        /// <returns></returns>
//        public static IDisposable BeginLifetimeScope(string tenantName, bool applyTenantSettings = false)
//        {
//            if (currentScopeDisposer.Value == null || currentScopeDisposer.Value.IsDisposed)
//            {
//                Engine.CurrentScope.Value = Engine.ContainerManager.Container.BeginLifetimeScope();

//                currentScopeDisposer.Value = new LifetimeScopeDispose(Engine.CurrentScope.Value);

//                var tenantLifeTimeScope = Engine.Resolve<TenantLifeTimeScope>();

//                if (applyTenantSettings)
//                {
//                    var info = Engine.Resolve<ITenantManager>().GetInfo();
//                    if (info != null)
//                    {
//                        if (info.Culture != null)
//                        {
//                            var ci = new CultureInfo(CultureInfoHelper.SetCultureInfo(info.Culture));
//                            CultureInfo.CurrentCulture = ci;
//                            CultureInfo.CurrentUICulture = ci;
//                        }

//                        tenantLifeTimeScope.Start(info.Subdomain);
//                    }
//                    else
//                    {
//                        tenantLifeTimeScope.Start(tenantName);
//                    }
//                }
//                else
//                {
//                    tenantLifeTimeScope.Start(tenantName);
//                }

//                //se não houver culture definida, define a padrão (situação ocorreu no jobs em ambientes linux/docker)
//                if (string.IsNullOrEmpty(CultureInfo.CurrentCulture?.Name))
//                {
//                    CultureInfoHelper.SetDefaultCultureInfo();
//                }

//                return currentScopeDisposer.Value;
//            }

//            return null;
//        }

//        /// <summary>
//        /// inicia um escopo onde o sistema não irá buscar por configurações específicas do tenant, apenas do environment.
//        /// </summary>
//        /// <returns></returns>
//        public static IDisposable BeginIgnoreTenantConfigs(bool ignoreTenantConfigs = true)
//        {
//            return new IgnoreTenantConfigScopeDisposable(ignoreTenantConfigs);
//        }

//        public static AsyncLocal<bool> IgnoreTenantConfigsScope = new AsyncLocal<bool>();

//        class IgnoreTenantConfigScopeDisposable : IDisposable
//        {
//            private bool originalValue;

//            public IgnoreTenantConfigScopeDisposable()
//            {
//                originalValue = IgnoreTenantConfigsScope.Value;
//                IgnoreTenantConfigsScope.Value = true;
//            }

//            public IgnoreTenantConfigScopeDisposable(bool ignoreValue)
//            {
//                originalValue = IgnoreTenantConfigsScope.Value;
//                IgnoreTenantConfigsScope.Value = ignoreValue;
//            }

//            public void Dispose()
//            {
//                IgnoreTenantConfigsScope.Value = originalValue;
//            }
//        }

//        #region RESOLVE 

//        public static T Resolve<T>()
//        {
//            return ContainerManager.Resolve<T>("", CurrentScope.Value);
//        }

//        public static T Resolve<T>(ILifetimeScope scope)
//        {
//            return ContainerManager.Resolve<T>("", scope);
//        }

//        public static T Resolve<T>(Dictionary<string, object> parameters)
//        {
//            var listParameters = new List<Parameter>();

//            foreach (var item in parameters)
//            {
//                listParameters.Add(new NamedParameter(item.Key, item.Value));
//            }

//            return ContainerManager.Container.Resolve<T>(listParameters);
//        }

//        public static object Resolve(Type type)
//        {
//            return ContainerManager.Resolve(type);
//        }

//        public static bool TryResolve<T>(out T instance) where T : class
//        {
//            return ContainerManager.Container.TryResolve(out instance);
//        }

//        public static bool TryResolve(Type type, out object instance)
//        {
//            return ContainerManager.Container.TryResolve(type, out instance);
//        }

//        public static object Resolve(Type type, params Type[] typeArguments)
//        {
//            return ContainerManager.Resolve(type.MakeGenericType(typeArguments));
//        }

//        public static bool TryResolve(Type type, out object instance, params Type[] typeArguments)
//        {
//            try
//            {
//                instance = ContainerManager.Resolve(type.MakeGenericType(typeArguments));

//                return true;
//            }
//            catch (Exception)
//            {
//                instance = null;

//                return false;
//            }
//        }

//        public static T[] ResolveAll<T>()
//        {
//            return ContainerManager.ResolveAll<T>();
//        }

//        public static string Get(string key)
//        {
//            if (_localizationProvider == null) return key;

//            return _localizationProvider.Get(key);
//        }

//        public static string GetByValue(string value)
//        {
//            if (_localizationProvider == null) return value;

//            return _localizationProvider.GetByValue(value);
//        }

//        public static string Get(string key, CultureInfo culture)
//        {
//            if (_localizationProvider == null) return key;

//            return _localizationProvider.Get(key, culture);
//        }

//        public static string Get(string key, params object[] args)
//        {
//            if (_localizationProvider == null) return key;

//            return string.Format(_localizationProvider.Get(key), args);
//        }

//        public static string Get(string key, CultureInfo culture, params object[] args)
//        {
//            if (_localizationProvider == null) return key;

//            return string.Format(_localizationProvider.Get(key, culture), args);
//        }

//        public static ContainerManager ContainerManager
//        {
//            get { return _containerManager; }
//        }


//        #endregion
//    }
//}


#endregion

using Autofac;
using System.Configuration;
using System.Collections.Specialized;
using Hub.Infrastructure.Extensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Hub.Infrastructure.DependencyInjection;
using Hub.Infrastructure.Autofac;
using Hub.Infrastructure.Database.Interfaces;
using System.Globalization;
using Autofac.Core;
using Hub.Infrastructure.Helpers;
using Hub.Infrastructure.Database;
using Hub.Infrastructure.Architecture.Localization.Interfaces;
using Hub.Infrastructure.Architecture.Tasks.Interfaces;
using System.Threading.Tasks;
using Hub.Infrastructure.Architecture.Tasks;
using Hub.Infrastructure.Architecture.Mapper.Interfaces;

namespace Hub.Infrastructure.Architecture
{
    public static class Engine
    {
        #region App Settings

        private static readonly AsyncLocal<NameValueCollection> AsyncLocalConfig = new();
        private static readonly object AppSettingsLock = new();
        private static NameValueCollection _configCollection;

        public static NameValueCollection AppSettings
        {
            get
            {
                if (_configCollection == null)
                {
                    lock (AppSettingsLock)
                    {
                        if (_configCollection == null)
                        {
                            var all = ConfigurationManager.AppSettings.Keys.Count > 0 ? new NameValueCollection(ConfigurationManager.AppSettings) : Environment.GetEnvironmentVariables().ToNameValueCollection();

                            foreach (var debugKey in all.AllKeys.Where(key => key.StartsWith("Debug:")))
                            {
                                var originalKey = debugKey.Replace("Debug:", "");
                                all[originalKey] = all[debugKey];
                            }

                            _configCollection = all;
                        }
                    }
                }

                if (AsyncLocalConfig.Value == null) return _configCollection;

                var allConfigs = new NameValueCollection(_configCollection);
                foreach (string key in AsyncLocalConfig.Value)
                {
                    allConfigs[key] = AsyncLocalConfig.Value[key];
                }

                return allConfigs;
            }
        }

        public static void SetThreadSetting(string key, string value)
        {
            AsyncLocalConfig.Value ??= new NameValueCollection();
            AsyncLocalConfig.Value[key] = value;
        }

        public static void RemoveThreadSetting(string key)
        {
            AsyncLocalConfig.Value?.Remove(key);
        }

        public static IDisposable SetScopedSetting(string key, string value)
        {
            var currentValue = AppSettings[key];
            SetThreadSetting(key, value);
            return new ScopeAppSetting(key, currentValue);
        }

        private class ScopeAppSetting : IDisposable
        {
            private readonly string _key;
            private readonly string _originalValue;

            public ScopeAppSetting(string key, string originalValue)
            {
                _key = key;
                _originalValue = originalValue;
            }

            public void Dispose()
            {
                SetThreadSetting(_key, _originalValue);
            }
        }

        #endregion

        #region Connection Strings

        public static string ConnectionString(string settingName)
        {
            return ConfigurationManager.ConnectionStrings[settingName]?.ConnectionString ?? AppSettings[$"ConnectionString-{settingName}"] ?? Environment.GetEnvironmentVariable($"ConnectionString-{settingName}");
        }

        #endregion

        #region Mapper

        private static List<IAutoMapperStartup> _autoMapperStartups;

        public static void RunAutoMapperStartups(IMapperConfigurationExpression cfg)
        {
            _autoMapperStartups?.ForEach(startup => startup.RegisterMaps(cfg));
        }

        public static void RegisterAutoMapperStartup(IAutoMapperStartup startup)
        {
            _autoMapperStartups ??= new List<IAutoMapperStartup>();
            _autoMapperStartups.Add(startup);
        }

        #endregion

        public static Assembly ExecutingAssembly { get; private set; }
        private static ContainerManager _containerManager;
        private static Action _initializeAction;
        private static ILocalizationProvider _localizationProvider;
        private static readonly AsyncLocal<ILifetimeScope> CurrentScope = new();
        private static readonly AsyncLocal<LifetimeScopeDispose> CurrentScopeDisposer = new();

        private class LifetimeScopeDispose : IDisposable
        {
            public ILifetimeScope Scope { get; }
            public bool IsDisposed { get; private set; }

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
            _initializeAction?.Invoke();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Initialize(EngineInitializationParameters parameters)
        {
            Initialize(
                parameters.ExecutingAssembly,
                parameters.NameProvider,
                parameters.StartupTasks,
                parameters.DependencyRegistrators,
                parameters.ConnectionStringBase,
                parameters.ContainerBuilder);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Initialize(
            Assembly executingAssembly,
            IEntityNameProvider nameProvider = null,
            IList<IStartupTask> tasks = null,
            IList<IDependencyConfiguration> dependencyRegistrars = null,
            ConnectionStringBaseVM connectionStringBase = null,
            ContainerBuilder containerBuilder = null)
        {
            ExecutingAssembly = executingAssembly;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            Singleton<IEntityNameProvider>.Instance = nameProvider ?? new DefaultNameProvider();
            Singleton<LoopTenantManager>.Instance = new LoopTenantManager();

            dependencyRegistrars ??= new List<IDependencyConfiguration> { new DependencyConfiguration() };

            _containerManager = new ContainerManager(dependencyRegistrars, containerBuilder);

            _initializeAction = () =>
            {
                tasks ??= new List<IStartupTask> { new StartupTask() };
                tasks.OrderBy(t => t.Order).ToList().ForEach(t => t.Execute());

                if (_autoMapperStartups?.Count > 0)
                {
                    var config = new MapperConfiguration(RunAutoMapperStartups);
                    Singleton<IMapper>.Instance = config.CreateMapper();
                }

                TryResolve(out _localizationProvider);

                if (connectionStringBase != null)
                {
                    Resolve<ConnectionStringBaseConfigurator>().Set(connectionStringBase);
                }

                if (TryResolve(out IOrmConfiguration ormConfiguration))
                {
                    ormConfiguration.Configure();
                }
            };

            if (_containerManager.Container != null)
            {
                _initializeAction.Invoke();
            }
        }

        public static IDisposable BeginLifetimeScope(bool preserveTenantScope = false)
        {
            if (CurrentScopeDisposer.Value == null || CurrentScopeDisposer.Value.IsDisposed)
            {
                var tenantName = preserveTenantScope ? Singleton<IEntityNameProvider>.Instance.TenantName() : null;

                CurrentScope.Value = _containerManager.Container.BeginLifetimeScope();
                CurrentScopeDisposer.Value = new LifetimeScopeDispose(CurrentScope.Value);

                if (preserveTenantScope)
                {
                    Resolve<TenantLifeTimeScope>().Start(tenantName);
                }

                return CurrentScopeDisposer.Value;
            }

            return null;
        }

        public static IDisposable BeginLifetimeScope(string tenantName, bool applyTenantSettings = false)
        {
            if (CurrentScopeDisposer.Value == null || CurrentScopeDisposer.Value.IsDisposed)
            {
                CurrentScope.Value = _containerManager.Container.BeginLifetimeScope();
                CurrentScopeDisposer.Value = new LifetimeScopeDispose(CurrentScope.Value);

                var tenantLifeTimeScope = Resolve<TenantLifeTimeScope>();

                if (applyTenantSettings)
                {
                    var info = Resolve<ITenantManager>().GetInfo();

                    if (info?.Culture != null)
                    {
                        var ci = new CultureInfo(CultureInfoHelper.SetCultureInfo(info.Culture));
                        CultureInfo.CurrentCulture = ci;
                        CultureInfo.CurrentUICulture = ci;
                    }

                    tenantLifeTimeScope.Start(info?.Subdomain ?? tenantName);
                }
                else
                {
                    tenantLifeTimeScope.Start(tenantName);
                }

                if (string.IsNullOrEmpty(CultureInfo.CurrentCulture?.Name))
                {
                    CultureInfoHelper.SetDefaultCultureInfo();
                }

                return CurrentScopeDisposer.Value;
            }

            return null;
        }

        public static IDisposable BeginIgnoreTenantConfigs(bool ignoreTenantConfigs = true)
        {
            return new IgnoreTenantConfigScopeDisposable(ignoreTenantConfigs);
        }

        private static readonly AsyncLocal<bool> IgnoreTenantConfigsScope = new();

        private class IgnoreTenantConfigScopeDisposable : IDisposable
        {
            private readonly bool _originalValue;

            public IgnoreTenantConfigScopeDisposable(bool ignoreValue)
            {
                _originalValue = IgnoreTenantConfigsScope.Value;
                IgnoreTenantConfigsScope.Value = ignoreValue;
            }

            public void Dispose()
            {
                IgnoreTenantConfigsScope.Value = _originalValue;
            }
        }

        #region Resolve

        public static T Resolve<T>() => _containerManager.Resolve<T>("", CurrentScope.Value);

        public static T Resolve<T>(ILifetimeScope scope) => _containerManager.Resolve<T>("", scope);

        public static T Resolve<T>(Dictionary<string, object> parameters)
        {
            var listParameters = parameters.Select(p => new NamedParameter(p.Key, p.Value)).ToList();
            return _containerManager.Container.Resolve<T>(listParameters);
        }

        public static object Resolve(Type type) => _containerManager.Resolve(type);

        public static bool TryResolve<T>(out T instance) where T : class => _containerManager.Container.TryResolve(out instance);

        public static bool TryResolve(Type type, out object instance) => _containerManager.Container.TryResolve(type, out instance);

        public static object Resolve(Type type, params Type[] typeArguments)
            => _containerManager.Resolve(type.MakeGenericType(typeArguments));

        public static bool TryResolve(Type type, out object instance, params Type[] typeArguments)
        {
            try
            {
                instance = _containerManager.Resolve(type.MakeGenericType(typeArguments));
                return true;
            }
            catch
            {
                instance = null;
                return false;
            }
        }

        public static T[] ResolveAll<T>() => _containerManager.ResolveAll<T>();

        public static string Get(string key) => _localizationProvider?.Get(key) ?? key;

        public static string GetByValue(string value) => _localizationProvider?.GetByValue(value) ?? value;

        public static string Get(string key, CultureInfo culture) => _localizationProvider?.Get(key, culture) ?? key;

        public static string Get(string key, params object[] args) => string.Format(_localizationProvider?.Get(key) ?? key, args);

        public static string Get(string key, CultureInfo culture, params object[] args)
            => string.Format(_localizationProvider?.Get(key, culture) ?? key, args);

        public static ContainerManager ContainerManager => _containerManager;

        #endregion
    }
}
