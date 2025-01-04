namespace Hub.Infrastructure.Database.Models
{
    public class Authentication
    {
        public string UserName { get; private set; }

        public string Password { get; private set; }

        public bool RememberMe { get; private set; }

        public object FingerPrint { get; private set; }

        public Authentication(string userName, string password, bool rememberMe, object fingerPrint = null)
        {
            UserName = userName;
            Password = password;
            RememberMe = rememberMe;
            FingerPrint = fingerPrint;
        }
    }

    public enum EAuthProvider
    {
        Form = 1,
        Api = 2
    }
}
