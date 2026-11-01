using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class GenreManager : IDisposable
    {
        private readonly GenreRepository _repository = new GenreRepository();

        public IList<Genre> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public Genre GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(Genre entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            DateTime now = DateTime.UtcNow;
            // sin campos automáticos
            entity.Id = 0;
            _repository.Add(entity);
            return errors;
        }

        public IList<string> Update(Genre entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            Genre existing = _repository.GetById(entity.Id);
            if (existing == null)
            {
                errors.Add("El registro ya no existe.");
                return errors;
            }

            DateTime now = DateTime.UtcNow;
            // sin campos automáticos
            _repository.Update(entity.Id, entity);
            return errors;
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        private static void Normalize(Genre entity)
        {
            entity.Name = ValidationRules.Clean(entity.Name);
            entity.Description = ValidationRules.Clean(entity.Description);
        }

        private IList<string> Validate(Genre entity)
        {
            var errors = new List<string>();
            ValidationRules.Required(errors, "Nombre", entity.Name, 100);
            ValidationRules.Optional(errors, "Descripción", entity.Description, 500);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
