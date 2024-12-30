namespace Hub.Infrastructure.Security.Interfaces
{
    public interface ISecurityProvider
    {
        //bool Authenticate(AuthenticationVM authenticationVM);

        void Authenticate(string token);
    }
}
