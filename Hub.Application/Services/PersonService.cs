using Hub.Domain.Entities;
using Hub.Infrastructure.Architecture.Localization;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Database.Models;
using Hub.Infrastructure.Exceptions;
using Hub.Infrastructure.Web;

namespace Hub.Application.Services
{
    public class PersonService : OrchestratorService<Person>
    {
        public PersonService(IRepository<Person> repository) : base(repository) { }

        private void Validate(Person entity)
        {
            if (string.IsNullOrEmpty(entity.Name))
            {
                throw new BusinessException(entity.DefaultRequiredMessage(e => e.Name));
            }

            if (string.IsNullOrEmpty(entity.Document))
            {
                throw new BusinessException(entity.DefaultRequiredMessage(e => e.Document));
            }

            if (Table.Any(u => u.Document == entity.Document && u.Id != entity.Id))
            {
                throw new BusinessException(entity.DefaultAlreadyRegisteredMessage(e => e.Document));
            }
        }

        private void ValidateInsert(Person entity)
        {
            Validate(entity);

        }

        public override long Insert(Person entity)
        {
            ValidateInsert(entity);

            using (var transaction = base._repository.BeginTransaction())
            {
                var ret = base._repository.Insert(entity);

                if (transaction != null) base._repository.Commit();

                return ret;
            }
        }

        public override void Update(Person entity)
        {
            Validate(entity);

            using (var transaction = base._repository.BeginTransaction())
            {
                base._repository.Update(entity);

                if (transaction != null) base._repository.Commit();
            }
        }

        public override void Delete(long id)
        {
            using (var transaction = base._repository.BeginTransaction())
            {
                base._repository.Delete(id);

                if (transaction != null) base._repository.Commit();
            }
        }

        public Person SavePerson(string document, string name, IList<OrganizationalStructure> structures, OrganizationalStructure ownerOrgStruct = null)
        {
            if (string.IsNullOrEmpty(document))
            {
                return null;
            }

            document = document.Replace(".", "").Replace("-", "").Replace("/", "");

            Person person = Table.FirstOrDefault(p => p.Document == document);

            if (person != null)
            {
                person.OrganizationalStructures.Clear();

                foreach (var structure in structures)
                {
                    person.OrganizationalStructures.Add(structure);
                }

                Update(person);
            } 
            else
            {
                person = new Person()
                {
                    Document = document,
                    Name = name,
                    OrganizationalStructures = structures,
                    OwnerOrgStruct = ownerOrgStruct
                };

                Insert(person);
            }

            return person;
        }      
    }
}
