using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class PublisherManager : IDisposable
    {
        private readonly PublisherRepository _repository = new PublisherRepository();

        public IList<Publisher> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public Publisher GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(Publisher entity)
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

        public IList<string> Update(Publisher entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            Publisher existing = _repository.GetById(entity.Id);
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

        private static void Normalize(Publisher entity)
        {
            entity.Name = ValidationRules.Clean(entity.Name);
            entity.Country = ValidationRules.Clean(entity.Country);
            entity.Website = ValidationRules.Clean(entity.Website);
            entity.Email = ValidationRules.Clean(entity.Email);
        }

        private IList<string> Validate(Publisher entity)
        {
            var errors = new List<string>();
            ValidationRules.Required(errors, "Nombre", entity.Name, 200);
            ValidationRules.Optional(errors, "País", entity.Country, 100);
            ValidationRules.Url(errors, "Sitio web", entity.Website, 255);
            ValidationRules.Email(errors, "Correo electrónico", entity.Email, false, 150);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
