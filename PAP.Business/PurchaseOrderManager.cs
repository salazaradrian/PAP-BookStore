using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class PurchaseOrderManager : IDisposable
    {
        private readonly PurchaseOrderRepository _repository = new PurchaseOrderRepository();
        private readonly SupplierRepository _supplierRepository = new SupplierRepository();

        public IList<PurchaseOrder> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public PurchaseOrder GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(PurchaseOrder entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            DateTime now = DateTime.UtcNow;
            entity.OrderDate = now;
            entity.Id = 0;
            _repository.Add(entity);
            return errors;
        }

        public IList<string> Update(PurchaseOrder entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            PurchaseOrder existing = _repository.GetById(entity.Id);
            if (existing == null)
            {
                errors.Add("El registro ya no existe.");
                return errors;
            }

            DateTime now = DateTime.UtcNow;
            entity.OrderDate = existing.OrderDate;
            _repository.Update(entity.Id, entity);
            return errors;
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public IList<LookupItem> GetSuppliersOptions()
        {
            return _supplierRepository.GetAll()
                .Select(x => new LookupItem(x.Id, x.Name))
                .OrderBy(i => i.Text)
                .ToList();
        }

        private static void Normalize(PurchaseOrder entity)
        {
            entity.Status = ValidationRules.Clean(entity.Status) ?? "Pending";
        }

        private IList<string> Validate(PurchaseOrder entity)
        {
            var errors = new List<string>();
            ValidationRules.Selected(errors, "Proveedor", entity.SupplierId);
            ValidationRules.Optional(errors, "Estado", entity.Status, 30);
            ValidationRules.Range(errors, "Costo total", entity.TotalCost, 0, 9999999999999);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
            _supplierRepository.Dispose();
        }
    }
}
