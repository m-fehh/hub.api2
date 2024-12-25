using Dapper;
using Hub.Infrastructure.Autofac;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.MultiTenant;
using Microsoft.Data.SqlClient;

namespace Hub.Infrastructure.Database
{
    public class DefaultOrmConfiguration : IOrmConfiguration
    {
        public void Configure()
        {
            var mapping = new TenantConfigurationSection { Mapeamentos = new NhConfigurationMapeamentoCollection() };
            var connectionStringAdmin = Engine.ConnectionString("adm");

            if (!string.IsNullOrEmpty(connectionStringAdmin))
            {
                var map = new TenantConfigurationMapeamento
                {
                    MapeamentoId = "default",
                    ConfigurationTenants = new TenantConfigurationCollection()
                };

                using (var connection = new SqlConnection(connectionStringAdmin))
                {
                    var query = @"
                        SELECT 
                            Id, 
                            Name, 
                            ConnectionString, 
                            IsActive, 
                            DefaultCulture
                        FROM 
                            Tenants
                        WHERE 
                            IsActive = 1"
                    ;

                    var tenants = connection.Query<Tenant>(query).ToList();

                    foreach (var item in tenants)
                    {
                        map.ConfigurationTenants.Add(new TenantConfigurationData
                        {
                            TenantId = item.Id.ToString(),
                            Name = item.Name,
                            ConnectionString = item.ConnectionString ?? Engine.ConnectionString("default"),
                            DefaultCulture = item.DefaultCulture,
                            SchemaDefault = $"sch{item.Id}",
                            ConnectionDriver = "MicrosoftDataSqlClientDriver",
                        });
                    }
                }

                if (Singleton<TenantConfigurationSection>.Instance?.AppPath != null)
                {
                    mapping.AppPath = Singleton<TenantConfigurationSection>.Instance?.AppPath;
                }
                else
                {
                    mapping.AppPath = AppDomain.CurrentDomain.BaseDirectory;
                }

            }

            Singleton<TenantConfigurationSection>.Instance = mapping;
        }
    }

    public class Tenant
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public bool IsActive { get; set; }
        public string DefaultCulture { get; set; }
        public string Schema { get; set; }
    }
}

