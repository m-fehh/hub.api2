using Hub.Domain;
using Hub.Infrastructure.Database.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hub.Infrastructure.Database
{
    public class DefaultOrmConfiguration : IOrmConfiguration
    {
        public void Configure(ConnectionStringBaseVM config)
        {
            string connectionString = config?.ConnectionString ?? GetConnectionString();

            var serviceProvider = new ServiceCollection().AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString)).BuildServiceProvider();
        }

        private string GetConnectionString()
        {
            return Engine.ConnectionString("DefaultConnection");
        }
    }
}




//using (var context = serviceProvider.GetService<ApplicationDbContext>())
//{
//    // Cria o banco de dados se ele não existir
//    context.Database.EnsureCreated();

//    // Verifica se a tabela 'tenants' existe
//    if (!context.Tenants.Any())
//    {
//        // Se a tabela estiver vazia, cria um tenant fake
//        var defaultTenant = new Tenant
//        {
//            Name = "Trainly Base",
//            Subdomain = "system",
//            IsActive = true,
//            CultureName = "pt-BR"
//        };

//        context.Tenants.Add(defaultTenant);
//        context.SaveChanges();
//    }

//    // Recupera todos os tenants existentes
//    var tenants = context.Tenants.ToList();

//    foreach (var tenant in tenants)
//    {
//        // Aqui você pode realizar a configuração específica para cada tenant
//        using (Engine.BeginLifetimeScope(tenant.Subdomain))
//        {
//            using (Engine.BeginIgnoreTenantConfigs(false))
//            {
//                var cs = Engine.ConnectionString("default");

//                // Adiciona as configurações específicas para o tenant
//                var tenantConfig = new NhConfigurationData
//                {
//                    TenantId = tenant.Subdomain,
//                    ConnectionString = cs,
//                    ConnectionProvider = "NHibernate.Connection.DriverConnectionProvider",  // Pode ser ajustado conforme a necessidade
//                    ConnectionDriver = "NHibernate.Driver.MicrosoftDataSqlClientDriver",  // Pode ser ajustado conforme a necessidade
//                    Dialect = "Hub.Infrastructure.Database.NhManagement.FMKSQLDIalect, Hub.Infrastructure", // Pode ser ajustado conforme a necessidade
//                    CurrentSessionContext = "async_local",
//                    UseSecondLevelCache = "false",
//                    UseQueryCache = "false",
//                    SchemaDefault = $"{tenant.Subdomain}",
//                    CacheProvider = "CoreDistributedCacheProvider"
//                };
//            }
//        }
//    }