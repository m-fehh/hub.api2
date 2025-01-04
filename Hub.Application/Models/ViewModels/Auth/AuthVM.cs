using Hub.Infrastructure.Database.Models;

namespace Hub.Application.Models.ViewModels.Auth
{
    public class AuthVM
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        public bool RememberMe { get; set; }

        public EAuthProvider Provider { get; set; }

        public FingerPrintVM FingerPrint { get; set; }
    }
}
