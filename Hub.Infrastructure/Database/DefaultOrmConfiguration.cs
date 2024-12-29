using Dapper;
using Hub.Infrastructure.Autofac;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.Models;
using Microsoft.Data.SqlClient;

namespace Hub.Infrastructure.Database
{
    public class DefaultOrmConfiguration : IOrmConfiguration
    {
        public void Configure()
        {
            var mapeamento = new ConfigurationTenant
            {
                Mapeamentos = new ConfigurationMapeamentoCollection()
            };

            var connectionStringAdmin = Engine.ConnectionString("adm");
            var baseConfigurator = Engine.Resolve<ConnectionStringBaseConfigurator>().Get();

            if (!string.IsNullOrEmpty(connectionStringAdmin))
            {
                var map = new ConfigurationMapeamento
                {
                    MapeamentoId = "default",
                    ConfigurationTenants = new ConfigurationDataCollection()
                };

                using (var connection = new SqlConnection(connectionStringAdmin))
                {
                    const string query = @"
                        SELECT 
                            Id, 
                            Name,
                            Subdomain,
                            ConnectionString, 
                            IsActive, 
                            DefaultCulture
                        FROM 
                            Tenants
                        WHERE 
                            IsActive = 1";

                    var clients = connection.Query<AdmClient>(query);

                    foreach (var client in clients)
                    {
                        using (Engine.BeginLifetimeScope(client.Subdomain))
                        {
                            using (Engine.BeginIgnoreTenantConfigs(false))
                            {
                                map.ConfigurationTenants.Add(new ConfigurationData
                                {
                                    TenantId = client.Id,
                                    TenantName = client.Name,
                                    Subdomain = client.Subdomain,
                                    ConnectionString = client.ConnectionString ?? Engine.ConnectionString("default"),
                                    SchemaDefault = $"{baseConfigurator.ConnectionStringBaseSchema}{client.Id}",
                                    Culture = client.DefaultCulture
                                });
                            }
                        }
                    }
                }

                if (Singleton<ConfigurationTenant>.Instance?.AppPath != null)
                {
                    //se o valor do AppPath já estiver preenchido, preserva.
                    //acontece que nas functions o appPath é setado diretamento no Startup, e rotinas de reconfiguração do ORM fazem passar por aqui novamente.
                    mapeamento.AppPath = Singleton<ConfigurationTenant>.Instance?.AppPath;
                }
                else
                {
                    mapeamento.AppPath = AppDomain.CurrentDomain.BaseDirectory;
                }

                mapeamento.Mapeamentos.Add(map);
            }

            Singleton<ConfigurationTenant>.Instance = mapeamento;
        }
    }
}