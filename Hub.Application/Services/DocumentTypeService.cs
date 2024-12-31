using AutoMapper;
using Hub.Application.ViewModels;
using Hub.Domain.Entities;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Web;

namespace Hub.Application.Services
{
    public class DocumentTypeService : CrudService<DocumentType>
    {
        public DocumentTypeService(IRepository<DocumentType> repository) : base(repository) { }

        public override long Insert(DocumentType entity)
        {
            using (var transaction = _repository.BeginTransaction())
            {
                var ret = _repository.Insert(entity);

                if (transaction != null)    _repository.Commit();

                return ret;
            }
        }

        public override void Update(DocumentType entity)
        {
            using (var transaction = base._repository.BeginTransaction())
            {
                _repository.Update(entity);

                if (transaction != null) _repository.Commit();
            }
        }

        public override void Delete(long id)
        {
            using (var transaction = _repository.BeginTransaction())
            {
                _repository.Delete(id);

                if (transaction != null) _repository.Commit();
            }
        }


        public List<DocumentTypeVM> ListDocumentType()
        {
            var items = Get(w => !w.Inactive, s => s).ToList();

            return Singleton<IMapper>.Instance.Map<List<DocumentTypeVM>>(items);
        }
    }
}
