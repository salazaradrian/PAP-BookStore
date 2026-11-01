using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class ShoppingCartManager : IDisposable
    {
        private readonly ShoppingCartRepository _repository = new ShoppingCartRepository();
        private readonly CustomerRepository _customerRepository = new CustomerRepository();

        public IList<ShoppingCart> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public ShoppingCart GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(ShoppingCart entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            DateTime now = DateTime.UtcNow;
            entity.CreatedAt = now;
            entity.UpdatedAt = now;
            entity.Id = 0;
            _repository.Add(entity);
            return errors;
        }

        public IList<string> Update(ShoppingCart entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            ShoppingCart existing = _repository.GetById(entity.Id);
            if (existing == null)
            {
                errors.Add("El registro ya no existe.");
                return errors;
            }

            DateTime now = DateTime.UtcNow;
            entity.CreatedAt = existing.CreatedAt;
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

        private static void Normalize(ShoppingCart entity)
        {
            // sin normalización
        }

        private IList<string> Validate(ShoppingCart entity)
        {
            var errors = new List<string>();
            ValidationRules.Selected(errors, "Cliente", entity.CustomerId);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
            _customerRepository.Dispose();
        }
    }
}
