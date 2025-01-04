using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Models;

namespace Hub.Infrastructure.Architecture.Security.Interfaces
{
    public interface ISecurityProvider
    {
        bool Authenticate(Authentication authenticationVM);

        void Authenticate(string token);

        bool Authorize(string role);

        //List<string> GetAuthorizedRoles(List<string> roles);
        IUserAccount GetCurrent();

        IProfileGroup GetCurrentProfile();

        long? GetCurrentId();

        long? GetCurrentProfileId();
    }
}
