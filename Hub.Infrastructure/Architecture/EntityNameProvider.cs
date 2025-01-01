using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Web;
using System.Collections.Concurrent;

namespace Hub.Infrastructure.Autofac
{
    public class EntityNameProvider : IEntityNameProvider
    {
        public static AsyncLocal<string> CurrentTenant = new AsyncLocal<string>();
        public static ConcurrentDictionary<string, string> subdomains = new ConcurrentDictionary<string, string>();
        private static readonly HttpClient _httpClient = new HttpClient();

        public string TenantName()
        {
            var defaultTenantName = "adm";
            try
            {
                if (!string.IsNullOrEmpty(CurrentTenant.Value) && CurrentTenant.Value != "adm")
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
                    var configFixedDomain = Engine.AppSettings["FixedDomain"];

                    if (!string.IsNullOrWhiteSpace(configFixedDomain))
                    {
                        return configFixedDomain;
                    }
                }

                if (HttpContextHelper.Current == null ||
                HttpContextHelper.Current.Request == null) return defaultTenantName;

                var baseUrl = string.Format("{0}://{1}", HttpContextHelper.Current.Request.Scheme, HttpContextHelper.Current.Request.Host.Value);

                //return TenantByUrl(baseUrl);
                return "";
            }
            catch (Exception)
            {
                return defaultTenantName;
            }
        }
    }
}
