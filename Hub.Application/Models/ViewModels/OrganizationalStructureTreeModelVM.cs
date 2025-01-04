namespace Hub.Application.Models.ViewModels
{
    public class OrganizationalStructureTreeModelVM
    {
        public OrganizationalStructureTreeModelVM()
        {
            Items = new List<OrganizationalStructureTreeModelVM>();
        }

        public long Id { get; set; }
        public string Description { get; set; }
        public long? FatherId { get; set; }
        public bool Inactive { get; set; }

        public List<OrganizationalStructureTreeModelVM> Items { get; set; }
    }
}
