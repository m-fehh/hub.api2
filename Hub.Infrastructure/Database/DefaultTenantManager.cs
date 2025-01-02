using Dapper;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Cache;
using Hub.Infrastructure.Autofac;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.Models;
using Microsoft.Data.SqlClient;

namespace Hub.Infrastructure.Database
{
    /// <summary>
    /// Classe para obtenção das informações do atual tenant da aplicação.
    /// </summary>
    public class DefaultTenantManager : ITenantManager
    {
        public ITenantInfo GetInfo()
        {
            var _tenantName = Singleton<IEntityNameProvider>.Instance.TenantName();

            Func<string, AdmClient> fn = (tenantName) =>
            {
                var connectionString = Engine.ConnectionString("adm");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ArgumentNullException(nameof(connectionString));
                }
                using (var connection = new SqlConnection(connectionString))
                {
                    var query = $@"
                        DECLARE @tenantName NVARCHAR(200) = '{tenantName}';
                        
                        IF EXISTS(SELECT 1 FROM sys.columns 
                                  WHERE Name = N'CultureName'
                                  AND Object_ID = Object_ID(N'adm.Client'))
                        BEGIN
                        	DECLARE @SQLString NVARCHAR(500);  
                        	DECLARE @ParmDefinition NVARCHAR(500);
                        
                            SET @SQLString= N'SELECT a.Id, a.Name, a.Subdomain, a.Logo,	a.CultureName FROM adm.Client a WHERE a.Subdomain = @tenantName';
                        	SET @ParmDefinition = N'@tenantName nvarchar(200)';
                        	EXECUTE sp_executesql @SQLString, @ParmDefinition, @tenantName; 
                        END
                        ELSE
                        BEGIN
                        	 SELECT
								Id,
                                Name,
                                Subdomain,
                                ConnectionString,
                                IsActive,
                                DefaultCulture
                        	FROM Tenants a
                        	WHERE a.Subdomain = @tenantName;
                        END
                        ";

                    var client = connection.QueryFirstOrDefault<AdmClient>(query);
                    return client;
                }
            };

            return Engine.Resolve<CacheManager>().CacheAction(() => fn(_tenantName), redisCacheTimeSeconds: 0, localCacheTimeSeconds: 60);
        }
    }
}
