using System.ComponentModel.DataAnnotations;

namespace Hub.Application.Models.ViewModels.Login
{
    public class LoginVM
    {
        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }

        public bool ChangePassTemporary { get; set; }

        /// <summary>
        /// Representa o JSON vindo do client-side
        /// </summary>
        public string FingerPrint { get; set; }

        /// <summary>
        /// indica se o login veio de forma integrada com o AD da EVUP (usuários internos)
        /// </summary>
        public bool? IsProvider { get; set; }
    }
}
