using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class SupplierManager : IDisposable
    {
        private readonly SupplierRepository _repository = new SupplierRepository();

        public IList<Supplier> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public Supplier GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(Supplier entity)
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

        public IList<string> Update(Supplier entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            Supplier existing = _repository.GetById(entity.Id);
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

        private static void Normalize(Supplier entity)
        {
            entity.Name = ValidationRules.Clean(entity.Name);
            entity.ContactName = ValidationRules.Clean(entity.ContactName);
            entity.Email = ValidationRules.Clean(entity.Email);
            entity.Phone = ValidationRules.Clean(entity.Phone);
            entity.Address = ValidationRules.Clean(entity.Address);
        }

        private IList<string> Validate(Supplier entity)
        {
            var errors = new List<string>();
            ValidationRules.Required(errors, "Nombre", entity.Name, 200);
            ValidationRules.Optional(errors, "Contacto", entity.ContactName, 150);
            ValidationRules.Email(errors, "Correo electrónico", entity.Email, false, 150);
            ValidationRules.Optional(errors, "Teléfono", entity.Phone, 30);
            ValidationRules.Optional(errors, "Dirección", entity.Address, 300);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
