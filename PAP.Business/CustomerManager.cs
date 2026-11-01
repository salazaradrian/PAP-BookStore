using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class CustomerManager : IDisposable
    {
        private readonly CustomerRepository _repository = new CustomerRepository();

        public IList<Customer> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public Customer GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(Customer entity, string password)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity, password, true);
            if (errors.Count > 0)
                return errors;

            DateTime now = DateTime.UtcNow;
            entity.PasswordHash = PasswordHasher.Hash(password);
            entity.CreatedAt = now;
            entity.UpdatedAt = now;
            entity.Id = 0;
            _repository.Add(entity);
            return errors;
        }

        public IList<string> Update(Customer entity, string password)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity, password, false);
            if (errors.Count > 0)
                return errors;

            Customer existing = _repository.GetById(entity.Id);
            if (existing == null)
            {
                errors.Add("El registro ya no existe.");
                return errors;
            }

            DateTime now = DateTime.UtcNow;
            entity.PasswordHash = string.IsNullOrEmpty(password) ? existing.PasswordHash : PasswordHasher.Hash(password);
            entity.CreatedAt = existing.CreatedAt;
            entity.UpdatedAt = now;
            _repository.Update(entity.Id, entity);
            return errors;
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        private static void Normalize(Customer entity)
        {
            entity.FirstName = ValidationRules.Clean(entity.FirstName);
            entity.LastName = ValidationRules.Clean(entity.LastName);
            entity.Email = ValidationRules.Clean(entity.Email);
            entity.Phone = ValidationRules.Clean(entity.Phone);
        }

        private IList<string> Validate(Customer entity, string password, bool isNew)
        {
            var errors = new List<string>();
            ValidationRules.Required(errors, "Nombre", entity.FirstName, 100);
            ValidationRules.Required(errors, "Apellido", entity.LastName, 100);
            ValidationRules.Email(errors, "Correo electrónico", entity.Email, true, 150);
            ValidationRules.Optional(errors, "Teléfono", entity.Phone, 30);
            ValidationRules.Password(errors, password, isNew);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
