using Autofac;
using System.Configuration;
using System.Collections.Specialized;
using Hub.Infrastructure.Extensions;
using AutoMapper;
using Hub.Infrastructure.Mapper;
using Microsoft.EntityFrameworkCore;
using Hub.Infrastructure.DependencyInjection.Interfaces;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Hub.Infrastructure.DependencyInjection;
using Hub.Infrastructure.Autofac;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Localization.Interfaces;
using System.Globalization;
using Autofac.Core;
using Hub.Infrastructure.Helpers;
using Hub.Infrastructure.Database;

namespace Hub.Infrastructure
{
    public static class Engine
    {
        #region APP SETTINGS 

        /// <summary>
        /// Permite substituir/criar uma configuração para o escopo atual
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetThreadSetting(string key, string value)
        {
            if (AsyncLocalConfigCollection.Value == null)
            {
                AsyncLocalConfigCollection.Value = new NameValueCollection();
            }

            if (AsyncLocalConfigCollection.Value.AllKeys.Any(c => c == key))
            {
                AsyncLocalConfigCollection.Value[key] = value;
            }
            else
            {
                AsyncLocalConfigCollection.Value.Add(key, value);
            }
        }

        public static void RemoveThreadSetting(string key)
        {
            if (AsyncLocalConfigCollection.Value == null)
            {
                return;
            }

            if (AsyncLocalConfigCollection.Value.AllKeys.Any(c => c == key))
            {
                AsyncLocalConfigCollection.Value.Remove(key);
            }
        }

        public class ScopeAppSetting : IDisposable
        {
            private readonly string key;
            private readonly string originalValue;

            public ScopeAppSetting(string key, string originalValue)
            {
                this.key = key;
                this.originalValue = originalValue;
            }

            public void Dispose()
            {
                SetThreadSetting(key, originalValue);
            }
        }
        public static IDisposable SetScopedSetting(string key, string value)
        {
            var currentValue = AppSettings[key];

            SetThreadSetting(key, value);

            return new ScopeAppSetting(key, currentValue);
        }

        static NameValueCollection ConfigCollection = null;

        static Dictionary<string, NameValueCollection> TenantConfigCollection = new Dictionary<string, NameValueCollection>();

        static AsyncLocal<NameValueCollection> AsyncLocalConfigCollection = new AsyncLocal<NameValueCollection>();

        private static object appSettingsLock = new object();

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
                            NameValueCollection all = null;

                            if (ConfigurationManager.AppSettings.Keys.Count != 0)
                            {
                                all = new NameValueCollection(ConfigurationManager.AppSettings);
                            }
                            else
                            {
                                all = new NameValueCollection(Environment.GetEnvironmentVariables().ToNameValueCollection());
                            }


                            //chaves de debug (usadas para substituir qualquer chave quando em modo Debug)
                            //exemplo de chave no local.settings.json: "Debug:elos-api-endpoint"
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

        #endregion

        #region CONNECTION STRINGS 

        public static string ConnectionString(string settingName)
        {
            var cs = ConfigurationManager.ConnectionStrings[settingName];

            if (cs != null)
                return cs.ConnectionString;
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

        #endregion

        #region MAPPER 

        private static List<IAutoMapperStartup> _autoMapperStartups;

        public static void RunAutoMapperStartups(IMapperConfigurationExpression cfg)
        {
            if (_autoMapperStartups == null) return;

            foreach (var item in _autoMapperStartups)
            {
                item.RegisterMaps(cfg);
            }
        }

        #endregion

        public static Assembly ExecutingAssembly { get; private set; }
        private static ContainerManager _containerManager;
        private static Action initializeAction = null;
        private static ILocalizationProvider _localizationProvider;
        public static AsyncLocal<ILifetimeScope> CurrentScope = new AsyncLocal<ILifetimeScope>();
        private static AsyncLocal<LifetimeScopeDispose> currentScopeDisposer = new AsyncLocal<LifetimeScopeDispose>();

        class LifetimeScopeDispose : IDisposable
        {
            public ILifetimeScope Scope { get; set; }

            public bool IsDisposed { get; set; }

            public LifetimeScopeDispose(ILifetimeScope scope)
            {
                this.Scope = scope;
                this.IsDisposed = false;
            }

            public void Dispose()
            {
                if (CurrentScope.Value == this.Scope)
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Initialize(EngineInitializationParameters engineInitializationParameters)
        {
            Initialize(executingAssembly: engineInitializationParameters.ExecutingAssembly, dependencyRegistrars: engineInitializationParameters.DependencyRegistrators, csb: engineInitializationParameters.ConnectionStringBase, containerBuilder: engineInitializationParameters.ContainerBuilder);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Initialize(Assembly executingAssembly, IList<IDependencyConfiguration> dependencyRegistrars = null, ConnectionStringBaseVM csb = null, ContainerBuilder containerBuilder = null)
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

                if (csb != null)
                {
                    Resolve<ConnectionStringBaseConfigurator>().Set(csb);
                }

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

        /// <summary>
        /// Inicia um novo ciclo de vida do injetor de dependências
        /// </summary>
        /// <param name="copyTenantName">define se irá executar o comando que passa o nome do tenant atual para o novo ciclo criado</param>
        /// <returns></returns>
        public static IDisposable BeginLifetimeScope(bool copyTenantName = false)
        {
            if (currentScopeDisposer.Value == null || currentScopeDisposer.Value.IsDisposed)
            {
                string tenantName = null;

                //if (copyTenantName)
                //{
                //    tenantName = Singleton<INhNameProvider>.Instance.TenantName();
                //}

                CurrentScope.Value = ContainerManager.Container.BeginLifetimeScope();

                currentScopeDisposer.Value = new LifetimeScopeDispose(CurrentScope.Value);

                if (copyTenantName)
                {
                    TenantLifeTimeScope.Start(tenantName);
                }

                return currentScopeDisposer.Value;
            }

            return null;
        }

        /// <summary>
        /// Inicia um novo ciclo de vida do injetor de dependências
        /// </summary>
        /// <param name="tenantName">nome do tenant para o ciclo de vida que será criado</param>
        /// <param name="forceTenantAndCulture"></param>
        /// <returns></returns>
        public static IDisposable BeginLifetimeScope(string tenantName, bool forceTenantAndCulture = false)
        {
            if (currentScopeDisposer.Value == null || currentScopeDisposer.Value.IsDisposed)
            {
                CurrentScope.Value = ContainerManager.Container.BeginLifetimeScope();

                currentScopeDisposer.Value = new LifetimeScopeDispose(CurrentScope.Value);

                if (forceTenantAndCulture)
                {
                    var info = Resolve<ITenantManager>().GetInfo();
                    if (info != null)
                    {
                        if (info.DefaultCulture != null)
                        {
                            var ci = new CultureInfo(CultureInfoHelper.SetCultureInfo(info.DefaultCulture));
                            CultureInfo.CurrentCulture = ci;
                            CultureInfo.CurrentUICulture = ci;
                        }

                        TenantLifeTimeScope.Start(info.Subdomain);
                    }
                    else
                    {
                        TenantLifeTimeScope.Start(tenantName);
                    }
                }
                else
                {
                    TenantLifeTimeScope.Start(tenantName);
                }

                //se não houver culture definida, define a padrão (situação ocorreu no jobs em ambientes linux/docker)
                if (string.IsNullOrEmpty(CultureInfo.CurrentCulture?.Name))
                {
                    CultureInfoHelper.SetDefaultCultureInfo();
                }

                return currentScopeDisposer.Value;
            }

            return null;
        }

        /// <summary>
        /// inicia um escopo onde o sistema não irá buscar por configurações específicas do tenant, apenas do environment.
        /// </summary>
        /// <returns></returns>
        public static IDisposable BeginIgnoreTenantConfigs(bool ignoreTenantConfigs = true)
        {
            return new IgnoreTenantConfigScopeDisposable(ignoreTenantConfigs);
        }

        public static AsyncLocal<bool> IgnoreTenantConfigsScope = new AsyncLocal<bool>();

        class IgnoreTenantConfigScopeDisposable : IDisposable
        {
            private bool originalValue;

            public IgnoreTenantConfigScopeDisposable()
            {
                originalValue = IgnoreTenantConfigsScope.Value;
                IgnoreTenantConfigsScope.Value = true;
            }

            public IgnoreTenantConfigScopeDisposable(bool ignoreValue)
            {
                originalValue = IgnoreTenantConfigsScope.Value;
                IgnoreTenantConfigsScope.Value = ignoreValue;
            }

            public void Dispose()
            {
                IgnoreTenantConfigsScope.Value = originalValue;
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
