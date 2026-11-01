using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class AuthorManager : IDisposable
    {
        private readonly AuthorRepository _repository = new AuthorRepository();

        public IList<Author> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public Author GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(Author entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            DateTime now = DateTime.UtcNow;
            entity.CreatedAt = now;
            entity.Id = 0;
            _repository.Add(entity);
            return errors;
        }

        public IList<string> Update(Author entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            Author existing = _repository.GetById(entity.Id);
            if (existing == null)
            {
                errors.Add("El registro ya no existe.");
                return errors;
            }

            DateTime now = DateTime.UtcNow;
            entity.CreatedAt = existing.CreatedAt;
            _repository.Update(entity.Id, entity);
            return errors;
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        private static void Normalize(Author entity)
        {
            entity.FirstName = ValidationRules.Clean(entity.FirstName);
            entity.LastName = ValidationRules.Clean(entity.LastName);
            entity.Biography = ValidationRules.Clean(entity.Biography);
            entity.Country = ValidationRules.Clean(entity.Country);
        }

        private IList<string> Validate(Author entity)
        {
            var errors = new List<string>();
            ValidationRules.Required(errors, "Nombre", entity.FirstName, 100);
            ValidationRules.Required(errors, "Apellido", entity.LastName, 100);
            ValidationRules.Optional(errors, "Biografía", entity.Biography, 4000);
            ValidationRules.Optional(errors, "País", entity.Country, 100);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
