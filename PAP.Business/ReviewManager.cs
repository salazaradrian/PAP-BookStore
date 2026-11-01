using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class ReviewManager : IDisposable
    {
        private readonly ReviewRepository _repository = new ReviewRepository();
        private readonly BookRepository _bookRepository = new BookRepository();
        private readonly CustomerRepository _customerRepository = new CustomerRepository();

        public IList<Review> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public Review GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(Review entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            DateTime now = DateTime.UtcNow;
            entity.CreatedAt = now;
            entity.Id = 0;
            _repository.Add(entity);
            return errors;
        }

        public IList<string> Update(Review entity)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            Review existing = _repository.GetById(entity.Id);
            if (existing == null)
            {
                errors.Add("El registro ya no existe.");
                return errors;
            }

            DateTime now = DateTime.UtcNow;
            entity.CreatedAt = existing.CreatedAt;
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

        public IList<LookupItem> GetCustomersOptions()
        {
            return _customerRepository.GetAll()
                .Select(x => new LookupItem(x.Id, x.FirstName + " " + x.LastName + " (" + x.Email + ")"))
                .OrderBy(i => i.Text)
                .ToList();
        }

        private static void Normalize(Review entity)
        {
            entity.Comment = ValidationRules.Clean(entity.Comment);
        }

        private IList<string> Validate(Review entity)
        {
            var errors = new List<string>();
            ValidationRules.Selected(errors, "Libro", entity.BookId);
            ValidationRules.Selected(errors, "Cliente", entity.CustomerId);
            ValidationRules.Range(errors, "Calificación (1 a 5)", entity.Rating, 1, 5);
            ValidationRules.Optional(errors, "Comentario", entity.Comment, 1000);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
            _bookRepository.Dispose();
            _customerRepository.Dispose();
        }
    }
}
