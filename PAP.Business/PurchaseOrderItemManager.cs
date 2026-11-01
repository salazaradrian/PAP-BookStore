using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class PurchaseOrderItemManager : IDisposable
    {
        private readonly PurchaseOrderItemRepository _repository = new PurchaseOrderItemRepository();
        private readonly PurchaseOrderRepository _purchaseOrderRepository = new PurchaseOrderRepository();
        private readonly BookRepository _bookRepository = new BookRepository();

        public IList<PurchaseOrderItem> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public PurchaseOrderItem GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(PurchaseOrderItem entity)
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

        public IList<string> Update(PurchaseOrderItem entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            PurchaseOrderItem existing = _repository.GetById(entity.Id);
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

        public IList<LookupItem> GetPurchaseOrdersOptions()
        {
            return _purchaseOrderRepository.GetAll()
                .Select(x => new LookupItem(x.Id, "Orden #" + x.Id + " - " + x.Supplier.Name))
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

        private static void Normalize(PurchaseOrderItem entity)
        {
            // sin normalización
        }

        private IList<string> Validate(PurchaseOrderItem entity)
        {
            var errors = new List<string>();
            ValidationRules.Selected(errors, "Orden de compra", entity.PurchaseOrderId);
            ValidationRules.Selected(errors, "Libro", entity.BookId);
            ValidationRules.Range(errors, "Cantidad", entity.Quantity, 1, 100000);
            ValidationRules.Range(errors, "Costo unitario", entity.UnitCost, 0, 9999999999999);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
            _purchaseOrderRepository.Dispose();
            _bookRepository.Dispose();
        }
    }
}
