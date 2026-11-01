using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class OrderManager : IDisposable
    {
        private readonly OrderRepository _repository = new OrderRepository();
        private readonly CustomerRepository _customerRepository = new CustomerRepository();
        private readonly AddressRepository _addressRepository = new AddressRepository();

        public IList<Order> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public Order GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(Order entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            DateTime now = DateTime.UtcNow;
            entity.OrderDate = now;
            entity.Total = entity.Subtotal + entity.TaxAmount + entity.ShippingCost;
            entity.UpdatedAt = now;
            entity.Id = 0;
            _repository.Add(entity);
            return errors;
        }

        public IList<string> Update(Order entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            Order existing = _repository.GetById(entity.Id);
            if (existing == null)
            {
                errors.Add("El registro ya no existe.");
                return errors;
            }

            DateTime now = DateTime.UtcNow;
            entity.OrderDate = existing.OrderDate;
            entity.Total = entity.Subtotal + entity.TaxAmount + entity.ShippingCost;
            entity.UpdatedAt = now;
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

        public IList<LookupItem> GetAddressesOptions()
        {
            return _addressRepository.GetAll()
                .Select(x => new LookupItem(x.Id, x.Customer.FirstName + " " + x.Customer.LastName + " - " + x.Street + ", " + x.City))
                .OrderBy(i => i.Text)
                .ToList();
        }

        private static void Normalize(Order entity)
        {
            if (entity.ShippingAddressId.HasValue && entity.ShippingAddressId.Value <= 0)
                entity.ShippingAddressId = null;
            entity.Status = ValidationRules.Clean(entity.Status) ?? "Pending";
            entity.Notes = ValidationRules.Clean(entity.Notes);
        }

        private IList<string> Validate(Order entity)
        {
            var errors = new List<string>();
            ValidationRules.Selected(errors, "Cliente", entity.CustomerId);
            ValidationRules.Optional(errors, "Estado", entity.Status, 30);
            ValidationRules.Range(errors, "Subtotal", entity.Subtotal, 0, 9999999999999);
            ValidationRules.Range(errors, "Impuestos", entity.TaxAmount, 0, 9999999999999);
            ValidationRules.Range(errors, "Costo de envío", entity.ShippingCost, 0, 9999999999999);
            ValidationRules.Optional(errors, "Notas", entity.Notes, 500);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
            _customerRepository.Dispose();
            _addressRepository.Dispose();
        }
    }
}
