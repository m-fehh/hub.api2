using Hub.Infrastructure.Autofac;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.Models;

namespace Hub.Infrastructure.Database
{
    /// <summary>
    /// Classe para obtenção das informações do atual tenant da aplicação.
    /// </summary>
    public class DefaultTenantManager : ITenantManager
    {
        public ITenantInfo GetInfo()
        {
           return new AdmClient();
        }
    }
}
