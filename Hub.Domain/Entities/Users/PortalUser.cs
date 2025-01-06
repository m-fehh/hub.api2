using Hub.Domain.Enums;
using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.Models.Tenant;
using Hub.Infrastructure.Extensions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub.Domain.Entities.Users
{
    [Table("PortalUser")]
    public class PortalUser : BaseEntity, IUserAccount, IEntityOrgStructOwned, IModificationControl
    {
        [Key]
        public override long Id { get; set; }

        public virtual long? PersonId { get; set; }

        [ForeignKey(nameof(PersonId))]
        public virtual Person Person { get; set; }

        [Required]
        [StringLength(150)]
        public virtual string Name { get; set; }

        [StringLength(150)]
        public virtual string Document { get; set; }

        [Required]
        [StringLength(150)]
        public virtual string Email { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string Login { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string Password { get; set; }

        [StringLength(50)]
        public virtual string TempPassword { get; set; }

        public virtual bool Inactive { get; set; }

        public virtual long ProfileId { get; set; }

        [NotMapped]
        [ForeignKey(nameof(ProfileId))]
        [JsonConverter(typeof(ConcreteTypeConverter<ProfileGroup>))]
        public virtual IProfileGroup Profile { get; set; }

        public virtual long UserRoleId { get; set; }

        [ForeignKey(nameof(UserRoleId))]
        public virtual UserRole UserRole { get; set; }

        public virtual long? DefaultOrgStructureId { get; set; }

        [NotMapped]
        [ForeignKey(nameof(DefaultOrgStructureId))]
        public virtual OrganizationalStructure DefaultOrgStructure { get; set; }

        public virtual long OwnerOrgStructId { get; set; }

        [NotMapped]
        [ForeignKey(nameof(OwnerOrgStructId))]
        public virtual OrganizationalStructure OwnerOrgStruct { get; set; }

        [NotMapped]
        public virtual ICollection<OrganizationalStructure> OrganizationalStructures { get; set; }

        [IgnoreLog]
        public virtual DateTime? CreationUTC { get; set; }

        [IgnoreLog]
        public virtual DateTime? LastUpdateUTC { get; set; }

        [IgnoreLog]
        public virtual bool ChangingPass { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string QrCodeInfo { get; set; }

        public virtual string? AreaCode { get; set; }

        public virtual string? PhoneNumber { get; set; }

        public virtual EGender? Gender { get; set; }

        public virtual EUserType? UserType { get; set; }

        public virtual DateTime? BirthDate { get; set; }

        public virtual DateTime? LastAccessDate { get; set; }

        public virtual DateTime? LastPasswordRecoverRequestDate { get; set; }

        [StringLength(100)]
        public virtual string? Keyword { get; set; }

        public virtual DateTime? LastUserUpdateUTC { get; set; }

        public virtual ICollection<IAccessRule> AllRules { get { return Profile?.Rules; } }

        public virtual bool IsFromApi { get; set; }

        public virtual string IpAddress { get; set; }

        public virtual DateTime? ExpirationDate { get; set; }

        public virtual EChatUserStatus? ChatUserStatus { get; set; }
    }
}







