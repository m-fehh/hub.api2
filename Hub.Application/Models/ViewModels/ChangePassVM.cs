using System.ComponentModel.DataAnnotations;

namespace Hub.Application.Models.ViewModels
{
    public class ChangePassVM
    {
        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required]
        [Display(Name = "OldPassword")]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "NewPassword")]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}
