using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.Models.Administrator;
using Hub.Infrastructure.Database.Services;

namespace Hub.Infrastructure.Database
{
    public class DefaultOrmConfiguration : IOrmConfiguration
    {
        public void Configure()
        {
            var configurationTenant = new ConfigurationTenant
            {
                Mapeamentos = new ConfigurationMapeamentoCollection()
            };

            var baseConfigurator = Engine.Resolve<ConnectionStringInitializer>().Get();

            var defaultMapeamento = new ConfigurationMapeamento
            {
                MapeamentoId = "default",
                ConfigurationTenants = new ConfigurationDataCollection()
            };

            var allClients = Engine.Resolve<TenantService>().GetAllClients();

            foreach (var client in allClients)
            {
                AddTenantConfiguration(client, baseConfigurator, defaultMapeamento);
            }

            configurationTenant.AppPath = Singleton<ConfigurationTenant>.Instance?.AppPath ?? AppDomain.CurrentDomain.BaseDirectory;
            configurationTenant.Mapeamentos.Add(defaultMapeamento);

            Singleton<ConfigurationTenant>.Instance = configurationTenant;
        }

        private void AddTenantConfiguration(Tenant client, ConnectionStringBaseVM baseConfigurator, ConfigurationMapeamento defaultMapeamento)
        {
            using (Engine.BeginLifetimeScope(client.Subdomain))
            {
                using (Engine.BeginIgnoreTenantConfigs(false))
                {
                    defaultMapeamento.ConfigurationTenants.Add(new ConfigurationData
                    {
                        TenantId = client.Id,
                        TenantName = client.Name,
                        Subdomain = client.Subdomain,
                        ConnectionString = client.ConnectionString ?? Engine.ConnectionString("default"),
                        SchemaDefault = $"{baseConfigurator.ConnectionStringBaseSchema}{client.Id}",
                        Culture = client.Culture
                    });
                }
            }
        }
    }
}
