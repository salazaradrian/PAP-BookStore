using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public partial class BookRepository
    {
        private IQueryable<Book> WithRelations()
        {
            return _set
                .Include(x => x.Category1)
                .Include(x => x.Genre1)
                .Include(x => x.Publisher1)
                .Include(x => x.Authors);
        }

        public override IEnumerable<Book> GetAll()
        {
            return WithRelations().OrderBy(x => x.Id).ToList();
        }

        public override Book GetById(int id)
        {
            return WithRelations().FirstOrDefault(x => x.Id == id);
        }

        public void Add(Book book, IEnumerable<int> authorIds)
        {
            SetAuthors(book, authorIds);
            _set.Add(book);
            Save();
        }

        public void Update(int id, Book values, IEnumerable<int> authorIds)
        {
            Book existing = GetById(id);
            if (existing == null)
                throw new System.InvalidOperationException("El registro ya no existe.");

            _context.Entry(existing).CurrentValues.SetValues(values);
            SetAuthors(existing, authorIds);
            Save();
        }

        public override void Delete(int id)
        {
            Book book = GetById(id);
            if (book == null)
                return;

            book.Authors.Clear();
            _set.Remove(book);
            Save();
        }

        // Many-to-many Book <-> Author has no entity class, so it is managed through the collection.
        private void SetAuthors(Book book, IEnumerable<int> authorIds)
        {
            book.Authors.Clear();
            if (authorIds == null)
                return;

            foreach (int authorId in authorIds.Distinct())
            {
                Author author = _context.Authors.Find(authorId);
                if (author != null)
                    book.Authors.Add(author);
            }
        }
    }
}
