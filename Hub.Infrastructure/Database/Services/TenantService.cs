using Hub.Infrastructure.Architecture.Cache;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Interfaces;
using Microsoft.Data.SqlClient;
using Dapper;
using Hub.Infrastructure.Extensions;
using Hub.Infrastructure.Database.Models.Administrator;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Hub.Infrastructure.Web.Services;

namespace Hub.Infrastructure.Database.Services
{
    public class TenantService : OrchestratorService<Tenant>
    {
        public TenantService(IRepository<Tenant> repository) : base(repository) { }

        /// <summary>
        /// Retrieves the tenant's subdomain or name based on the given URL.
        /// </summary>
        /// <param name="url">The URL to process.</param>
        /// <returns>The tenant's subdomain or name.</returns>
        public string Get(string url)
        {
            string NormalizeUrl(string url) => Regex.Replace(url.Replace("https://", "").Replace("http://", "").Replace(".hml", "").Replace(".dev", ""),@"\.com\.\w{1,3}", "");

            var normalizedUrl = NormalizeUrl(url);
            var urlParts = normalizedUrl.Split(".");
            var allClients = GetAllClients();

            // Find tenant by subdomain
            var client = allClients.FirstOrDefault(c => urlParts.Contains(c.Subdomain));
            if (client != null)
            {
                return client.Subdomain;
            }

            // Find tenant by URL mappings
            var allMaps = GetAllTenantMaps();
            var tenantMap = allMaps.FirstOrDefault(a => a.Url == url || url.Contains(a.Url));
            if (tenantMap != null)
            {
                return tenantMap.Tenant;
            }

            // Check subdomain variations
            foreach (var urlPart in urlParts)
            {
                var hifenParts = urlPart.Split('-');
                client = allClients.FirstOrDefault(c => hifenParts.Contains(c.Subdomain));
                if (client != null)
                {
                    return client.Subdomain;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Retrieves all tenant mappings.
        /// </summary>
        private List<TenantBindingVM> GetAllTenantMaps()
        {
            // Defina a expressão como uma função anônima
            Func<List<TenantBindingVM>> fn = () => Engine.Resolve<IRepository<TenantBinding>>().Table.Select(t => new TenantBindingVM(t.TenantName, t.Url)).ToList();

            var cacheOptions = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return Engine.Resolve<CacheManager>().CacheAction(() => fn(), localCacheTimeSeconds: 0, redisCacheTimeSeconds: 300, options: () => cacheOptions);
        }

        /// <summary>
        /// Retrieves all active clients (tenants).
        /// </summary>
        public List<Tenant> GetAllClients()
        {
            var fn = () =>
            {
                var connectionString = Engine.ConnectionString("adm");

                using (var connection = new SqlConnection(connectionString))
                {
                    var query = @"
                                SELECT
                                	[Id],
                                	[Name],
                                	[Subdomain],
                                	[Inative],
                                	[Culture],
                                	[ConnectionString]
                                FROM 
                                	[Admin].[Tenants]
                                WHERE
                                	[Inative] = 0";

                    return connection.Query<Tenant>(query).ToList();
                }
            };

            return Engine.Resolve<CacheManager>().CacheAction(() => fn(), localCacheTimeSeconds: 0, redisCacheTimeSeconds: 300);
        }

        /// <summary>
        /// ViewModel for Tenant Binding.
        /// </summary>
        private class TenantBindingVM
        {
            public TenantBindingVM(string tenant, string url)
            {
                Tenant = tenant;
                Url = url;
            }

            public string Tenant { get; set; }
            public string Url { get; set; }
        }
    }
}





