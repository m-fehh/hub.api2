using Hub.Domain.Entities.Users;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Web;

namespace Hub.Application.Services
{
    /// <summary>
    /// Serviço para armazenar informações diversas no momento do login
    /// <see href="https://dev.azure.com/evuptec/EVUP/_workitems/edit/17365/">Link do PBI</see>
    /// </summary>
    public class PortalUserFingerprintService : OrchestratorSimpleService<PortalUserFingerprint>
    {
        public PortalUserFingerprintService(IRepository<PortalUserFingerprint> repository) : base(repository) { }
    }
}
