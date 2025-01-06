using Dapper;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Cache;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.Models.Administrator;
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

            Func<string, Tenant> fn = (tenantName) =>
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
                        
                        IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS  WHERE COLUMN_NAME = N'Culture'  AND TABLE_SCHEMA = N'Admin' AND TABLE_NAME = N'Tenants')
                        BEGIN
                        	DECLARE @SQLString NVARCHAR(500);  
                        	DECLARE @ParmDefinition NVARCHAR(500);
                        
                            SET @SQLString= N'SELECT [Id], [Name], [Subdomain], [Inative], [Culture], [ConnectionString] FROM [Admin].[Tenants] WHERE [Subdomain] = @tenantName';
                        	SET @ParmDefinition = N'@tenantName nvarchar(200)';
                        	EXECUTE sp_executesql @SQLString, @ParmDefinition, @tenantName; 
                        END
                        ELSE
                        BEGIN
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
                                [Subdomain] = @tenantName;
                        END
                        ";

                    return connection.QueryFirstOrDefault<Tenant>(query);
                }
            };

            return Engine.Resolve<CacheManager>().CacheAction(() => fn(_tenantName), redisCacheTimeSeconds: 0, localCacheTimeSeconds: 60);
        }
    }
}


