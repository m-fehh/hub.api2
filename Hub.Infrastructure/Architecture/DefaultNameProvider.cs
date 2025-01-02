using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Cache;
using Hub.Infrastructure.Database.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Hub.Infrastructure.Autofac
{
    public class DefaultNameProvider : IEntityNameProvider
    {
        private static readonly HttpContextAccessor _httpContextAccessor = new HttpContextAccessor();
        private static readonly HttpClient _httpClient = new HttpClient();

        public string TenantName()
        {
            var fixedDomain = Engine.Resolve<TenantLifeTimeScope>().CurrentTenantName;

            if (!string.IsNullOrEmpty(fixedDomain))
            {
                return fixedDomain;
            }

            if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.Request == null)
            {
                return "default";
            }

            var baseUrl = string.Format("{0}://{1}", _httpContextAccessor.HttpContext.Request.Scheme, _httpContextAccessor.HttpContext.Request.Host.Value);

            return TenantByUrl(baseUrl);
        }

        public string TenantByUrl(string url)
        {
            Func<string, string> fn = (string url) =>
            {
                using (Engine.BeginIgnoreTenantConfigs())
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        var configServer = Engine.AppSettings["evup_config_url"];

                        if (string.IsNullOrWhiteSpace(configServer))
                        {
                            configServer = "https://config.evup.com.br";
                        }
                        url = url.Replace("https://", "");

                        string result = _httpClient.GetStringAsync($"{configServer}/api/Tenant/Get?url={url}").Result;

                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }
                    }
                }

                return "default";
            };

            using (Engine.BeginIgnoreTenantConfigs())
            {
                return Engine.Resolve<CacheManager>().CacheAction(() => fn(url), CacheManager.EnvironmentLevel, localCacheTimeSeconds: 60, redisCacheTimeSeconds: 0);
            }
        }
    }
}
