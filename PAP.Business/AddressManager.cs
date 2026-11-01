using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class AddressManager : IDisposable
    {
        private readonly AddressRepository _repository = new AddressRepository();
        private readonly CustomerRepository _customerRepository = new CustomerRepository();

        public IList<Address> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public Address GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(Address entity)
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

        public IList<string> Update(Address entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            Address existing = _repository.GetById(entity.Id);
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

        public IList<LookupItem> GetCustomersOptions()
        {
            return _customerRepository.GetAll()
                .Select(x => new LookupItem(x.Id, x.FirstName + " " + x.LastName + " (" + x.Email + ")"))
                .OrderBy(i => i.Text)
                .ToList();
        }

        private static void Normalize(Address entity)
        {
            entity.AddressType = ValidationRules.Clean(entity.AddressType) ?? "Shipping";
            entity.Street = ValidationRules.Clean(entity.Street);
            entity.City = ValidationRules.Clean(entity.City);
            entity.State = ValidationRules.Clean(entity.State);
            entity.PostalCode = ValidationRules.Clean(entity.PostalCode);
            entity.Country = ValidationRules.Clean(entity.Country);
        }

        private IList<string> Validate(Address entity)
        {
            var errors = new List<string>();
            ValidationRules.Selected(errors, "Cliente", entity.CustomerId);
            ValidationRules.Required(errors, "Tipo", entity.AddressType, 20);
            if (entity.AddressType != "Shipping" && entity.AddressType != "Billing")
                errors.Add("El valor de 'Tipo' no es válido.");
            ValidationRules.Required(errors, "Calle", entity.Street, 200);
            ValidationRules.Required(errors, "Ciudad", entity.City, 100);
            ValidationRules.Optional(errors, "Estado / Provincia", entity.State, 100);
            ValidationRules.Required(errors, "Código postal", entity.PostalCode, 20);
            ValidationRules.Required(errors, "País", entity.Country, 100);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
            _customerRepository.Dispose();
        }
    }
}
