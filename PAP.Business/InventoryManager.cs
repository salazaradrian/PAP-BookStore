using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class InventoryManager : IDisposable
    {
        private readonly InventoryRepository _repository = new InventoryRepository();
        private readonly BookRepository _bookRepository = new BookRepository();

        public IList<Inventory> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public Inventory GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(Inventory entity)
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

        public IList<string> Update(Inventory entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            Inventory existing = _repository.GetById(entity.Id);
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

        public IList<LookupItem> GetBooksOptions()
        {
            return _bookRepository.GetAll()
                .Select(x => new LookupItem(x.Id, x.Title))
                .OrderBy(i => i.Text)
                .ToList();
        }

        private static void Normalize(Inventory entity)
        {
            // sin normalización
        }

        private IList<string> Validate(Inventory entity)
        {
            var errors = new List<string>();
            ValidationRules.Selected(errors, "Libro", entity.BookId);
            ValidationRules.Range(errors, "Cantidad en stock", entity.QuantityInStock, 0, 100000000);
            ValidationRules.Range(errors, "Nivel de reorden", entity.ReorderLevel, 0, 100000000);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
            _bookRepository.Dispose();
        }
    }
}
