using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hub.Infrastructure.Web.Attributes.ToApplication
{
    public class PasswordValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var prop = validationContext.ObjectType.GetProperty("Id");

            if (prop != null)
            {
                var id = (long?)prop.GetValue(validationContext.ObjectInstance);

                if ((id == null || id == 0) && value == null)
                {
                    return new ValidationResult(string.Format(ErrorMessage, validationContext.DisplayName));
                }
            }
            else if (value == null)
            {
                return new ValidationResult(string.Format(ErrorMessage, validationContext.DisplayName));
            }

            return null;
        }
    }
}
