using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class OrderItemManager : IDisposable
    {
        private readonly OrderItemRepository _repository = new OrderItemRepository();
        private readonly OrderRepository _orderRepository = new OrderRepository();
        private readonly BookRepository _bookRepository = new BookRepository();

        public IList<OrderItem> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public OrderItem GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(OrderItem entity)
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

        public IList<string> Update(OrderItem entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            OrderItem existing = _repository.GetById(entity.Id);
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

        public IList<LookupItem> GetOrdersOptions()
        {
            return _orderRepository.GetAll()
                .Select(x => new LookupItem(x.Id, "Pedido #" + x.Id + " - " + x.Customer.FirstName + " " + x.Customer.LastName))
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

        private static void Normalize(OrderItem entity)
        {
            // sin normalización
        }

        private IList<string> Validate(OrderItem entity)
        {
            var errors = new List<string>();
            ValidationRules.Selected(errors, "Pedido", entity.OrderId);
            ValidationRules.Selected(errors, "Libro", entity.BookId);
            ValidationRules.Range(errors, "Cantidad", entity.Quantity, 1, 100000);
            ValidationRules.Range(errors, "Precio unitario", entity.UnitPrice, 0, 9999999999999);
            ValidationRules.Range(errors, "Descuento", entity.Discount, 0, 9999999999999);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
            _orderRepository.Dispose();
            _bookRepository.Dispose();
        }
    }
}
