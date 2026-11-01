using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class RoleManager : IDisposable
    {
        private readonly RoleRepository _repository = new RoleRepository();

        public IList<Role> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public Role GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(Role entity)
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

        public IList<string> Update(Role entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            Role existing = _repository.GetById(entity.Id);
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

        private static void Normalize(Role entity)
        {
            entity.Name = ValidationRules.Clean(entity.Name);
        }

        private IList<string> Validate(Role entity)
        {
            var errors = new List<string>();
            ValidationRules.Required(errors, "Nombre", entity.Name, 50);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
