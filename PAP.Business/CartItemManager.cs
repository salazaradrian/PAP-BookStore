using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class CartItemManager : IDisposable
    {
        private readonly CartItemRepository _repository = new CartItemRepository();
        private readonly ShoppingCartRepository _shoppingCartRepository = new ShoppingCartRepository();
        private readonly BookRepository _bookRepository = new BookRepository();

        public IList<CartItem> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public CartItem GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(CartItem entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            DateTime now = DateTime.UtcNow;
            entity.AddedAt = now;
            entity.Id = 0;
            _repository.Add(entity);
            return errors;
        }

        public IList<string> Update(CartItem entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            CartItem existing = _repository.GetById(entity.Id);
            if (existing == null)
            {
                errors.Add("El registro ya no existe.");
                return errors;
            }

            DateTime now = DateTime.UtcNow;
            entity.AddedAt = existing.AddedAt;
            _repository.Update(entity.Id, entity);
            return errors;
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public IList<LookupItem> GetShoppingCartsOptions()
        {
            return _shoppingCartRepository.GetAll()
                .Select(x => new LookupItem(x.Id, "Carrito #" + x.Id + " - " + x.Customer.FirstName + " " + x.Customer.LastName))
                .OrderBy(i => i.Text)
                .ToList();
        }

        public IList<LookupItem> GetBooksOptions()
        {
            return _bookRepository.GetAll()
                .Select(x => new LookupItem(x.Id, x.Title))
                .OrderBy(i => i.Text)
                .ToList();
        }

        private static void Normalize(CartItem entity)
        {
            // sin normalización
        }

        private IList<string> Validate(CartItem entity)
        {
            var errors = new List<string>();
            ValidationRules.Selected(errors, "Carrito", entity.CartId);
            ValidationRules.Selected(errors, "Libro", entity.BookId);
            ValidationRules.Range(errors, "Cantidad", entity.Quantity, 1, 100000);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
            _shoppingCartRepository.Dispose();
            _bookRepository.Dispose();
        }
    }
}
