using Hub.Infrastructure.Architecture.Cache;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Web;
using System.Collections.Concurrent;
using Hub.Infrastructure.Database.Services;

namespace Hub.Application
{
    public class HubTenantNameProvider : IEntityNameProvider
    {
        public static AsyncLocal<string> CurrentTenant = new AsyncLocal<string>();

        public static ConcurrentDictionary<string, string> subdomains = new ConcurrentDictionary<string, string>();

        public string TenantName()
        {
            string? defaultTenantName = "admin";

            try
            {
                if (!string.IsNullOrEmpty(CurrentTenant.Value) && CurrentTenant.Value != "admin")
                {
                    return CurrentTenant.Value;
                }

                var fixedDomain = Engine.Resolve<TenantLifeTimeScope>().CurrentTenantName;

                if (!string.IsNullOrEmpty(fixedDomain)) return fixedDomain;

                if (!string.IsNullOrEmpty(CurrentTenant.Value))
                {
                    return CurrentTenant.Value;
                }

                using (Engine.BeginIgnoreTenantConfigs())
                {
                    string? configFixedDomain = Engine.AppSettings["FixedDomain"];

                    if (!string.IsNullOrWhiteSpace(configFixedDomain))
                    {
                        return configFixedDomain;
                    }
                }

                if (HttpContextHelper.Current == null ||
                HttpContextHelper.Current.Request == null) return defaultTenantName;

                string? baseUrl = string.Format("{0}://{1}", HttpContextHelper.Current.Request.Scheme, HttpContextHelper.Current.Request.Host.Value);

                return TenantByUrl(baseUrl);
            }
            catch (Exception)
            {
                return defaultTenantName;
            }
        }

        public string TenantByUrl(string url)
        {

            Func<string, string> fn = (url) =>
            {
                using (Engine.BeginIgnoreTenantConfigs())
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        string result = Engine.Resolve<TenantService>().Get(url);

                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }
                    }
                }

                return "admin";
            };
            using (Engine.BeginIgnoreTenantConfigs())
            {
                return Engine.Resolve<CacheManager>().CacheAction(() => fn(url), CacheManager.EnvironmentLevel, localCacheTimeSeconds: 60, redisCacheTimeSeconds: 0);
            }
        }

        public void SetCurrentTenant(string url)
        {
            using (Engine.BeginIgnoreTenantConfigs())
            {
                string? configFixedDomain = Engine.AppSettings["FixedDomain"];

                if (!string.IsNullOrWhiteSpace(configFixedDomain))
                {
                    CurrentTenant.Value = configFixedDomain;
                    return;
                }
            }

            CurrentTenant.Value = TenantByUrl(url);
        }
    }
}
