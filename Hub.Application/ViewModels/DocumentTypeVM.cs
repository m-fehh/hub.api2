using Hub.Infrastructure.Internacionalization.Enums;
using System.ComponentModel.DataAnnotations;

namespace Hub.Application.ViewModels
{
    public class DocumentTypeVM
    {
        public long? Id { get; set; }

        [Display(Name = "Abbreviation")]
        public virtual string Abbrev { get; set; }

        [Display(Name = "Name")]
        public virtual string Name { get; set; }

        [Display(Name = "Mask")]
        public virtual string Mask { get; set; }

        [Display(Name = "CodeForTaxInvoice")]
        public virtual string CodeForTaxInvoice { get; set; }

        [Display(Name = "MinLength")]
        public virtual long? MinLength { get; set; }

        [Display(Name = "MaxLength")]
        public virtual long? MaxLength { get; set; }

        [Display(Name = "SpecialDocumentValidation")]
        public virtual ESpecialDocumentValidation? SpecialDocumentValidation { get; set; }

        [Display(Name = "Inactive")]
        public virtual bool Inactive { get; set; }
    }
}
