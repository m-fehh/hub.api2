namespace Hub.Infrastructure.Database.Models.Helpers
{
    public class AuthDetails
    {
        public string UserName { get; private set; }

        public string Password { get; private set; }

        public bool RememberMe { get; private set; }

        public object FingerPrint { get; private set; }

        public AuthDetails(string userName, string password, bool rememberMe, object fingerPrint = null)
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

    /// <summary>
    /// Destinado a armazenar informações diversas no momento do login do ELOS
    /// <see href="https://dev.azure.com/evuptec/EVUP/_workitems/edit/17365">Link do work item</see>
    /// </summary>
    public class FingerPrintVM
    {
        public string OS { get; set; }
        public string BrowserName { get; set; }
        public string BrowserInfo { get; set; }
        public string IpAddress { get; set; }

        public override string ToString()
        {
            return $"OS: {OS} / BrowserName: {BrowserName} / BrowserInfo: {BrowserInfo} / IP : {IpAddress}";
        }
    }

    public class LoginCredentialsVM
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }

        /// <summary>
        /// Aumenta o tempo de vida das credenciais (apenas para o provedor de login Form)
        /// </summary>
        public bool RememberMe { get; set; }

        public EAuthProvider Provider { get; set; }

        public FingerPrintVM FingerPrint { get; set; }
    }
}
