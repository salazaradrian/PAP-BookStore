using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class PaymentManager : IDisposable
    {
        private readonly PaymentRepository _repository = new PaymentRepository();
        private readonly OrderRepository _orderRepository = new OrderRepository();

        public IList<Payment> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public Payment GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(Payment entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            DateTime now = DateTime.UtcNow;
            entity.PaymentDate = now;
            entity.Id = 0;
            _repository.Add(entity);
            return errors;
        }

        public IList<string> Update(Payment entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            Payment existing = _repository.GetById(entity.Id);
            if (existing == null)
            {
                errors.Add("El registro ya no existe.");
                return errors;
            }

            DateTime now = DateTime.UtcNow;
            entity.PaymentDate = existing.PaymentDate;
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

        private static void Normalize(Payment entity)
        {
            entity.PaymentMethod = ValidationRules.Clean(entity.PaymentMethod);
            entity.Status = ValidationRules.Clean(entity.Status) ?? "Completed";
            entity.TransactionRef = ValidationRules.Clean(entity.TransactionRef);
        }

        private IList<string> Validate(Payment entity)
        {
            var errors = new List<string>();
            ValidationRules.Selected(errors, "Pedido", entity.OrderId);
            ValidationRules.Required(errors, "Método de pago", entity.PaymentMethod, 30);
            ValidationRules.Range(errors, "Monto", entity.Amount, 0, 9999999999999);
            ValidationRules.Optional(errors, "Estado", entity.Status, 30);
            ValidationRules.Optional(errors, "Referencia de transacción", entity.TransactionRef, 100);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
            _orderRepository.Dispose();
        }
    }
}
