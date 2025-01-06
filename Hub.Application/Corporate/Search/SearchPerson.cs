using Hub.Application.Corporate.Interfaces;
using Hub.Domain.Entities.Users;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Web.Attributes;

namespace Hub.Application.Corporate.Search
{
    [NoFreeSearch]
    public class SearchPerson : ISearchItem
    {
        public string SearchName
        {
            get { return "Person"; }
        }

        public List<ISearchResult> Get(string searchTerm, int pageSize, int pageNum, string extraCondition = null)
        {
            var repository = Engine.Resolve<IRepository<Person>>();

            var currentOrgStructId = Engine.Resolve<IHubCurrentOrganizationStructure>().Get();

            var currentDomain = Engine.Resolve<IHubCurrentOrganizationStructure>().GetCurrentDomain(currentOrgStructId);

            if (string.IsNullOrWhiteSpace(currentDomain))
            {
                currentDomain = currentOrgStructId;
            }
            else
            {
                currentDomain = $"({currentDomain})";
            }

            var query = repository.Table.Where(w => (w.Name.Contains(searchTerm) || w.Document.Contains(searchTerm)));

            return query
                .Where(w => w.OrganizationalStructures.Any(o => o.Tree.Contains(currentDomain)))
                .Skip(pageSize * (pageNum - 1))
                .Take(pageSize)
                .ToList()
                .Select(s => new SearchResult()
                {
                    id = s.Id,
                    text = s.Document + " - " + s.Name
                }).ToList<ISearchResult>();
        }

        public long GetCount(string searchTerm, string extraCondition = null)
        {
            return 10;
        }

        public SearchResult GetById(long id)
        {
            var repository = Engine.Resolve<IRepository<Person>>();

            return repository.Table
                .Where(u => u.Id == id)
                .ToList()
                .Select(u => new SearchResult()
                {
                    id = u.Id,
                    text = u.Document + " - " + u.Name
                }).FirstOrDefault();
        }

        public string RegistrationName
        {
            get { return Engine.Get("Person"); }
        }


    }
}
