using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class BookManager : IDisposable
    {
        private readonly PublisherRepository _publisherRepository = new PublisherRepository();
        private readonly CategoryRepository _categoryRepository = new CategoryRepository();
        private readonly GenreRepository _genreRepository = new GenreRepository();
        private readonly AuthorRepository _authorRepository = new AuthorRepository();

        public IList<Book> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public Book GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(Book entity, IList<int> authorIds)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            SyncLegacyColumns(entity, authorIds);
            DateTime now = DateTime.UtcNow;
            entity.CreatedAt = now;
            entity.UpdatedAt = now;
            entity.Id = 0;
            _repository.Add(entity, authorIds);
            return errors;
        }

        public IList<string> Update(Book entity, IList<int> authorIds)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity);
            if (errors.Count > 0)
                return errors;

            Book existing = _repository.GetById(entity.Id);
            if (existing == null)
            {
                errors.Add("El registro ya no existe.");
                return errors;
            }

            SyncLegacyColumns(entity, authorIds);
            DateTime now = DateTime.UtcNow;
            entity.CreatedAt = existing.CreatedAt;
            entity.UpdatedAt = now;
            _repository.Update(entity.Id, entity, authorIds);
            return errors;
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public IList<LookupItem> GetPublishersOptions()
        {
            return _publisherRepository.GetAll()
                .Select(x => new LookupItem(x.Id, x.Name))
                .OrderBy(i => i.Text)
                .ToList();
        }

        public IList<LookupItem> GetCategoriesOptions()
        {
            return _categoryRepository.GetAll()
                .Select(x => new LookupItem(x.Id, x.Name))
                .OrderBy(i => i.Text)
                .ToList();
        }

        public IList<LookupItem> GetGenresOptions()
        {
            return _genreRepository.GetAll()
                .Select(x => new LookupItem(x.Id, x.Name))
                .OrderBy(i => i.Text)
                .ToList();
        }

        public IList<LookupItem> GetAuthorsOptions()
        {
            return _authorRepository.GetAll()
                .Select(x => new LookupItem(x.Id, x.FirstName + " " + x.LastName))
                .OrderBy(i => i.Text)
                .ToList();
        }

        private static void Normalize(Book entity)
        {
            entity.Title = ValidationRules.Clean(entity.Title);
            entity.Name = ValidationRules.Clean(entity.Name) ?? ValidationRules.Clean(entity.Title);
            entity.ISBN = ValidationRules.Clean(entity.ISBN);
            if (entity.PublisherId.HasValue && entity.PublisherId.Value <= 0)
                entity.PublisherId = null;
            if (entity.CategoryId.HasValue && entity.CategoryId.Value <= 0)
                entity.CategoryId = null;
            if (entity.GenreId.HasValue && entity.GenreId.Value <= 0)
                entity.GenreId = null;
            entity.Language = ValidationRules.Clean(entity.Language);
            entity.CoverImageUrl = ValidationRules.Clean(entity.CoverImageUrl);
            entity.Description = ValidationRules.Clean(entity.Description);
        }

        private IList<string> Validate(Book entity)
        {
            var errors = new List<string>();
            ValidationRules.Required(errors, "Título", entity.Title, 200);
            ValidationRules.Optional(errors, "Nombre interno (opcional)", entity.Name, 200);
            ValidationRules.Optional(errors, "ISBN", entity.ISBN, 20);
            ValidationRules.Optional(errors, "Idioma", entity.Language, 50);
            if (entity.Pages.HasValue)
                ValidationRules.Range(errors, "Páginas", entity.Pages.Value, 1, 100000);
            ValidationRules.Range(errors, "Precio", entity.Price, 0, 9999999999999);
            if (entity.Height.HasValue)
                ValidationRules.Range(errors, "Alto", entity.Height.Value, 0, 99999999);
            if (entity.Width.HasValue)
                ValidationRules.Range(errors, "Ancho", entity.Width.Value, 0, 99999999);
            ValidationRules.Url(errors, "URL de la portada", entity.CoverImageUrl, 500);
            ValidationRules.Optional(errors, "Descripción", entity.Description, 4000);
            return errors;
        }

        // Los campos de texto antiguos (Category, Publisher, Genre, Author) se mantienen sincronizados.
        private void SyncLegacyColumns(Book entity, IList<int> authorIds)
        {
            entity.Category = entity.CategoryId.HasValue ? _categoryRepository.GetById(entity.CategoryId.Value)?.Name : null;
            entity.Publisher = entity.PublisherId.HasValue ? _publisherRepository.GetById(entity.PublisherId.Value)?.Name : null;
            entity.Genre = entity.GenreId.HasValue ? _genreRepository.GetById(entity.GenreId.Value)?.Name : null;

            var names = (authorIds ?? new List<int>())
                .Distinct()
                .Select(id => _authorRepository.GetById(id))
                .Where(a => a != null)
                .Select(a => a.FirstName + " " + a.LastName)
                .ToList();
            entity.Author = names.Count == 0 ? null : string.Join(", ", names);
        }

        public void Dispose()
        {
            _repository.Dispose();
            _publisherRepository.Dispose();
            _categoryRepository.Dispose();
            _genreRepository.Dispose();
            _authorRepository.Dispose();
        }
    }
}
