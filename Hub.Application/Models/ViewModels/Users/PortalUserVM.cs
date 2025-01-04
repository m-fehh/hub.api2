using Hub.Domain.Enums;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Architecture.Mapper.Interfaces;
using Hub.Infrastructure.Web.Attributes;
using Hub.Infrastructure.Web.Attributes.ToApplication;
using System.ComponentModel.DataAnnotations;

namespace Hub.Application.Models.ViewModels.Users
{
    public class PortalUserVM : IModelEntity
    {
        public long? Id { get; set; }

        public string SerializedOldValue { get; set; }

        [Required]
        [Display(Name = "FullName")]
        public string Name { get; set; }

        [Required]
        [DocumentValidation]
        [Display(Name = "Document")]
        public string Document { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [PasswordValidation]
        [Display(Name = "TempPassword")]
        [DataType(DataType.Password)]
        public string TempPassword { get; set; }

        [Display(Name = "Inactive")]
        public bool Inactive { get; set; }

        [Required]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        //[Required]
        //[Display(Name = "Profile")]
        //public long? Profile_Id { get; set; }

        //[Display(Name = "DefaultOrgStructure")]
        //public long? DefaultOrgStructure_Id { get; set; }

        //    [Display(Name = "OrganizationalStructure")]
        //    public List<string> OrganizationalStructuresId { get; set; }

        [Display(Name = "QrCode")]
        public string QrCodeInfo { get; set; }

        //public string FormattedDocument
        //{
        //    get
        //    {
        //        return Document.ToCpfCnpj();
        //    }
        //}

        [Display(Name = "AreaCode")]
        public string AreaCode { get; set; }

        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Display(Name = "UserType")]
        public EUserType? UserType { get; set; }

        [Display(Name = "UserType")]
        public string UserTypeDescription
        {
            get
            {
                return UserType?.ToString();
            }
        }

        [StringLength(100)]
        [Display(Name = "UserKeyword")]
        public string Keyword { get; set; }

        [Display(Name = "LastAccessDate")]
        public DateTime? LastAccessDate { get; set; }

        [Display(Name = "ExpirationDate")]
        public DateTime? ExpirationDate { get; set; }

        [Display(Name = "ChatUserStatus")]
        public EChatUserStatus? ChatUserStatus { get; set; }

        [Display(Name = "ChatUserStatus")]
        public string ChatUserStatusDescription
        {
            get
            {
                if (ChatUserStatus != null)
                {
                    return Engine.Get(ChatUserStatus?.ToString());
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
