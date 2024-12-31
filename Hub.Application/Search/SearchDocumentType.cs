using Hub.Domain.Entities;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Interfaces;

namespace Hub.Application.Search
{
    public class SearchDocumentType : ISearchItem
    {
        public string SearchName
        {
            get { return "DocumentType"; }
        }

        public List<ISearchResult> Get(string searchTerm, int pageSize, int pageNum, string extraCondition = null)
        {
            var repository = Engine.Resolve<IRepository<DocumentType>>();

            IQueryable<DocumentType> query;

            if (searchTerm.Any(char.IsDigit))
            {
                query = repository.Table
                .Where(u =>
                     !u.Inactive
                    && u.Abbrev.StartsWith(searchTerm));
            }
            else
            {
                query = repository.Table
                    .Where(u =>
                         !u.Inactive);
            }

            return
                query.Take(pageSize)
            .Select(u => new SearchResult()
            {
                id = u.Id,
                text = u.Abbrev,
                CustomHtmlFormat = u.Abbrev
            }).ToList<ISearchResult>();
        }

        public SearchResult GetById(long id)
        {
            var repository = Engine.Resolve<IRepository<DocumentType>>();

            return repository.Table
                .Where(u => u.Id == id)
                .ToList()
                .Select(u => new SearchResult()
                {
                    id = u.Id,
                    text = u.Abbrev,
                    CustomHtmlFormat = u.Abbrev
                }).FirstOrDefault();
        }

        public long GetCount(string searchTerm, string extraCondition = null)
        {
            var repository = Engine.Resolve<IRepository<DocumentType>>();

            return repository.Table
                .Where(u => !u.Inactive)
                .ToList()
                .Count();
        }

        public string RegistrationName
        {
            get { return Engine.Get("DocumentType"); }
        }

        public string RegistrationAddress
        {
            get { return "~/DocumentType"; }
        }
    }
}
