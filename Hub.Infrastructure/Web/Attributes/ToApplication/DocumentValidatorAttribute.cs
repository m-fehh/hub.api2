using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Internacionalization;
using System.ComponentModel.DataAnnotations;

namespace Hub.Infrastructure.Web.Attributes.ToApplication
{
    public class DocumentValidationAttribute : ValidationAttribute
    {
        public DocumentValidationAttribute() : base(Engine.Get("DocumentInvalid")) { }

        //public override bool IsValid(object value)
        //{
        //    if (value == null) return true;

        //    return value.ToString().ValidateDocument();
        //}

        //public bool IsValid(object value, DocumentType documentType)
        //{
        //    if (value == null) return true;

        //    return value.ToString().ValidateDocumentByType(documentType);
        //}
    }
}
