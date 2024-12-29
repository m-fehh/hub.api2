using Dapper;
using Hub.Infrastructure.Autofac;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.Models;
using Microsoft.Data.SqlClient;

namespace Hub.Infrastructure.Database.MultiTenant
{
    public class TenantProvider : ITenantProvider
    {
        public const string DefaultSchemaName = "lala";
        private readonly string _defaultConnectionString;
        public readonly Dictionary<string, AdmClient> _tenantConfigurations;

        public string? CurrentTenant => TenantLifeTimeScope.CurrentTenant;

        public string DbSchemaName => (CurrentTenant != null) ? (_tenantConfigurations.TryGetValue(CurrentTenant, out var config) ? $"sch{config.Id}" : DefaultSchemaName) : DefaultSchemaName;

        public string ConnectionString
        {
            get
            {
                if (CurrentTenant != null && _tenantConfigurations.TryGetValue(CurrentTenant, out var config))
                {
                    return config.ConnectionString ?? _defaultConnectionString;
                }

                return _defaultConnectionString;
            }
        }

        public List<AdmClient> Tenants => _tenantConfigurations.Values.ToList();

        public TenantProvider()
        {
            _defaultConnectionString = Engine.ConnectionString("default");

            // Aqui fazemos a consulta aos tenants via Dapper
            using (var connection = new SqlConnection(Engine.ConnectionString("adm")))
            {
                connection.Open();

                // Consultando os tenants e suas configurações (ajuste a consulta conforme necessário)
                var query = @"
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

                var tenants = connection.Query<AdmClient>(query).ToList();

                // Convertendo os dados para o formato de dicionário
                _tenantConfigurations = tenants.ToDictionary(tenant => tenant.Subdomain, tenant => tenant);
            }

            if (_tenantConfigurations == null || !_tenantConfigurations.Any())
            {
                throw new InvalidOperationException("Nenhum tenant encontrado no banco de dados.");
            }
        }
    }
}

