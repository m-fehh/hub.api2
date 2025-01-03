using Hub.Infrastructure.Architecture;
using System.ComponentModel.DataAnnotations;

namespace Hub.Infrastructure.ViewModels
{
    public class OrganizationalStructureVM
    {
        public long? Id { get; set; }

        [Display(Name = "Abbrev")]
        public string Abbrev { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Inactive")]
        public bool Inactive { get; set; }

        [Display(Name = "AppearInMobileApp")]
        public bool AppearInMobileApp { get; set; }

        [Display(Name = "IsRoot")]
        public bool IsRoot { get; set; }

        [Display(Name = "IsLeaf")]
        public bool IsLeaf { get; set; }

        [Display(Name = "IsDomain")]
        public bool IsDomain { get; set; }

        [Display(Name = "Father_Description")]
        public long? Father_Id { get; set; }

        [Display(Name = "Father_Description")]
        public string Father_Description { get; set; }

        [Display(Name = "TypeDescription")]
        public string TypeDescription
        {
            get
            {
                if (IsLeaf) return Engine.Get("Establishment");
                if (IsDomain) return Engine.Get("DomainGroup");
                if (IsRoot) return Engine.Get("Root");

                return null;
            }
        }

        public string Tree { get; set; }
    }
}
